using StreamingClient.Base.Model.OAuth;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Twitch.Base;
using Twitch.Base.Models.NewAPI.Channels;
using Twitch.Base.Models.NewAPI.Users;

namespace Integrations
{
    public static class TwitchIntegration
    {
        const string ClientID = "jaxknsd82i923iqjeizvy6tzyz7t52";
        const string ClientSecret = "1f4hmc72plsytw2tub0fciga6ed31g";
        const string SuccessResponseHTMLBody = "<html><body>You're logged now. Please close this page.<body></html>";
        const string TwitchAuthBaseURL = "https://id.twitch.tv/oauth2/authorize";
        const string SuccesRedirectURL = "http://localhost:8919";
        const string AuthorizationScopes = "user:read:email";

        public static string AuthorizationCode { get; private set; }
        public static bool IsAuthorizing { get; private set; }
        public static bool IsAuthorized { get; private set; }
        public static bool IsConnecting { get; private set; }
        public static bool IsConnected { get; private set; }
        public static UserModel CurrentUser { get; private set; }
        public static ChannelInformationModel CurrentChannel { get; private set; }

        public delegate void TwitchConnectionHandler();
        public delegate void TwitchConnectionSuccessHandler(UserModel currentUser, ChannelInformationModel currentChannel);

        public static event TwitchConnectionHandler OnTwitchConnecting;
        public static event TwitchConnectionHandler OnTwitchDisconnected;
        public static event TwitchConnectionHandler OnTwitchConnectFail;
        public static event TwitchConnectionSuccessHandler OnTwitchConnected;

        private static OAuthTokenModel oauthModel;
        private static TwitchConnection connection;

        private static async Task ProcessConnection(HttpListenerContext listenerContext)
        {
            HttpStatusCode statusCode = HttpStatusCode.Unauthorized;
            string result = string.Empty;

            string token = HttpUtility.ParseQueryString(listenerContext.Request.Url.Query).Get("code");
            if (!string.IsNullOrEmpty(token))
            {
                statusCode = HttpStatusCode.OK;
                result = SuccessResponseHTMLBody;
                AuthorizationCode = token;
            }

            listenerContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            listenerContext.Response.StatusCode = (int)statusCode;
            listenerContext.Response.StatusDescription = statusCode.ToString();

            byte[] buffer = Encoding.UTF8.GetBytes(result);
            await listenerContext.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        public static async void Authorize()
        {
            IsAuthorizing = true;
            OnTwitchConnecting?.Invoke();
            var httpListener = new HttpListener();
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "client_id", ClientID },
                { "redirect_uri", SuccesRedirectURL },
                { "response_type", "code" },
                { "scope", AuthorizationScopes}
            };

                FormUrlEncodedContent content = new FormUrlEncodedContent(parameters.AsEnumerable());

                string twitchAuthURL = $"{TwitchAuthBaseURL}?{await content.ReadAsStringAsync()}";

                httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                httpListener.Prefixes.Add(SuccesRedirectURL + "/");
                httpListener.Start();

                ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = twitchAuthURL, UseShellExecute = true };
                Process.Start(startInfo);

                await Task.Factory.StartNew(async () =>
                {
                    while (httpListener != null && httpListener.IsListening)
                    {
                        HttpListenerContext context = httpListener.GetContext();

                        if (httpListener.IsListening)
                            await ProcessConnection(context);

                        IsAuthorized = context.Response.StatusCode == (int)HttpStatusCode.OK;
                    }
                }, TaskCreationOptions.LongRunning);


                var con = await GetConnectionAsync();
                CurrentUser = await con?.NewAPI.Users.GetCurrentUser();
                if (CurrentUser != null && CurrentUser.id != null)
                    CurrentChannel = await con?.NewAPI.Channels.GetChannelInformation(CurrentUser);
                else
                    throw new Exception("Unknow error when get current twitch user");
            }
            catch (Exception ex)
            {
                OnTwitchConnectFail?.Invoke();
                UnityEngine.Debug.LogException(ex);
            }

            httpListener.Stop();
            IsAuthorizing = false;

            if (!IsConnected && CurrentUser != null)
            {
                IsConnected = true;
                OnTwitchConnected?.Invoke(CurrentUser, CurrentChannel);
            }
        }

        private static void SetNewConnection(TwitchConnection twitchConnection)
        {
            if (twitchConnection != null && connection != twitchConnection)
            {
                connection = twitchConnection;
                oauthModel = connection.GetOAuthTokenCopy();

                if (!IsConnected && CurrentUser != null)
                {
                    IsConnected = true;
                    OnTwitchConnected?.Invoke(CurrentUser, CurrentChannel);
                }
            }
        }

        private static async Task<TwitchConnection> GetConnectionAsync()
        {
            try
            {
                if (oauthModel != null && oauthModel.ExpirationDateTime > DateTime.Now)
                    return connection;

                if (oauthModel != null && (oauthModel.ExpirationDateTime <= DateTime.Now))
                    SetNewConnection(await TwitchConnection.ConnectViaOAuthToken(oauthModel));

                if (oauthModel == null && !string.IsNullOrEmpty(AuthorizationCode))
                {
                    var newConnection = await TwitchConnection.ConnectViaAuthorizationCode(ClientID, ClientSecret, AuthorizationCode, redirectUrl: SuccesRedirectURL);
                    SetNewConnection(newConnection);
                }
            }
            catch (Exception ex)
            {
                connection = null;
                oauthModel = null;
                UnityEngine.Debug.LogError(ex.Message);
                if (IsConnected)
                {
                    IsConnected = false;
                    OnTwitchDisconnected?.Invoke();
                }
                else
                {
                    OnTwitchConnectFail?.Invoke();
                }
            }
            return connection;
        }

    }
}