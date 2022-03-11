using UnityEngine;

namespace Controllers.TwitchAPI
{
    public class TwitchController : MonoBehaviour
    {
public void OnEnable()
{
    //Register events
    Integrations.TwitchIntegration.OnTwitchConnecting += TwitchIntegration_OnTwitchConnecting;
    Integrations.TwitchIntegration.OnTwitchConnectFail += TwitchIntegration_OnTwitchConnectFail;
    Integrations.TwitchIntegration.OnTwitchConnected += TwitchIntegration_OnTwitchConnected;
    Integrations.TwitchIntegration.OnTwitchDisconnected += TwitchIntegration_OnTwitchDisconnected;
}

public void OnDisable()
{
    //Unregister events
    Integrations.TwitchIntegration.OnTwitchConnecting -= TwitchIntegration_OnTwitchConnecting;
    Integrations.TwitchIntegration.OnTwitchConnectFail -= TwitchIntegration_OnTwitchConnectFail;
    Integrations.TwitchIntegration.OnTwitchConnected -= TwitchIntegration_OnTwitchConnected;
    Integrations.TwitchIntegration.OnTwitchDisconnected -= TwitchIntegration_OnTwitchDisconnected;
}

private void TwitchIntegration_OnTwitchDisconnected()
{
#if DEBUG
    Debug.Log("Twitch is disconnected");
#endif
    //Put here your twitch disconnection logic
}

private void TwitchIntegration_OnTwitchConnected(Twitch.Base.Models.NewAPI.Users.UserModel currentUser, Twitch.Base.Models.NewAPI.Channels.ChannelInformationModel currentChannel)
{
#if DEBUG
    Debug.Log("Twitch connection sucessfull");
    Debug.Log($"UserLogin:{currentUser.login};UserDisplayName:{currentUser.display_name};UserEmail:{currentUser.email}");
    Debug.Log($"ChannelLanguage:{currentChannel.broadcaster_language};ChannelGame:{currentChannel.game_name};ChannelTitle:{currentChannel.title}");
#endif
    //Put here your twitch connected logic
}

private void TwitchIntegration_OnTwitchConnectFail()
{
#if DEBUG
    Debug.Log("Twitch connection failed");
#endif
    //Put here your twitch connection fail logic
}

private void TwitchIntegration_OnTwitchConnecting()
{
#if DEBUG
    Debug.Log("Twitch start connection process");
#endif
    //Put here your twitch connecting logic
}

        public void Connect()
            => Integrations.TwitchIntegration.Authorize();
    }
}