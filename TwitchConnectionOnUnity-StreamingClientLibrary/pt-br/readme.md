# Twitch Connection on Unity

Veja como fazer a integra��o da autoriza��o e a autentica��o da twitch em sua aplica��o ou jogo criado na Unity.

## **Preparando os recursos e depend�ncias**

Para iniciarmos, � preciso antes garantir que estamos com todas as etapas preparatorias foram executadas.

### **Cadastrar uma aplica��o na Twitch**

� preciso realizar o cadastro da sua aplica��o na Twitch. Para isto use os seguintes passos:

#### *Acesse o [console de desenvolvedor da Twitch](https://dev.twitch.tv/)*

#### *Se ainda n�o estiver logado no console, clique em [Log in with Twitch](https://dev.twitch.tv/login?next=https%3A%2F%2Fdev.twitch.tv%2F)*

#### *Se esta for a primeira vez que voc� acessa esta p�gina, autorize o v�nculo da conta a partir do bot�o 'Autorizar'*

#### *J� autenticado na pagina inicial da Twitch Developers, clique em [Your Console](https://dev.twitch.tv/console)*

#### *No console de desenvolvedor da Twitch, v� ate a aba [Applications](https://dev.twitch.tv/console/apps)*

#### *Nesta tela voc� ir� visualizar todas suas aplica��es registradas. Para registrar uma nova aplica��o, clique em [+ Register your Application](https://dev.twitch.tv/console/apps/create)*

#### *Aqui voc� vai precisar informar o Nome, a(s) URL(s) de retorno do Token OAuth e a Categoria em que sua aplica��o se enquadra. Cadastre os valores que voc� achar pertinente. Abaixo vamos descrever os valores utilizados neste treinamento:*

#### - ***Name**: TTVIntegrationWithUnity*

#### - ***OAuth** Redirect URLs: http://localhost:8919* (URL Padr�o da biblioteca utilizada, depois voc� ver� como customiz�-la)

#### - ***Category**: Game Integration*

#### *Ap�s preenchido os campos, clique em Create*

#### *Agora que foi redirecionado para a lista de Aplica��es criadas, clique em "Manage" para a aplica��o correspondente*

#### *V� ao final da p�gina e clique em "New Secret" e ent�o clique em OK*

#### *Agora anote os valores do seu 'Cient ID' e tamb�m do seu 'Client Secret' (Anote em um local seguro. Assim que voc� fechar esta p�gina, n�o ter� mais onde obter o valor de Client Secret gerado, te obrigando a gerar um novo. Assim todas aplica��es que usavam seu Secret antigo deixariam de funcionar.)*

#### **Baixar e Compilar a biblioteca *StreamingClientLibrary***

Neste treinamento, ser� utilizado a biblioteca [StreamingClientLibrary](https://github.com/SaviorXTanren/StreamingClientLibrary) desenvolvida por [Matthew Olivo](https://github.com/SaviorXTanren), um mero desenvolvedor do time do XBOX na microsoft, e tamb�m um dos fundadores e lider de desenvolvimento do Mixitup, um BOT para a Mixer e que foi migrado (em apenas uma noite) para a Twitch ap�s o encerramento de suas atividades.

Como usaremos esta biblioteca na Unity, a utiliza��o de pacotes Nuget n�o auxiliam muito, por isto a abordagem ser� a de baixar os codigos fontes atualizados, compilar a biblioteca em uma unica DLL, e ent�o utilizar ela na sua aplica��o Unity.

#### *Baixe o c�digo fonte atualizado da [biblioteca StreamingClientLibrary](https://github.com/SaviorXTanren/StreamingClientLibrary/archive/refs/heads/master.zip)*

#### *Descompacte os arquivos em algum diret�rio de sua escolha*

#### *Navegue at� a pasta onde voc� extraiu os arquivos, e ent�o clicando com o bot�o direito  na pasta "StreamingClient.Base" clique em "Abrir no terminal do Windows"*

#### *Execute o comando 'dotnet publish Twitch\Twitch.Base'*

#### *Pronto, na pasta Twitch\Twitch.Base\bin\Debug\netstandard2.0\publish haver� a DLL e suas depend�ncias que iremos utilizar na Unity*

## **Criando a autoriza��o Twitch na Unity**

Crie um novo projeto ou utilize um j� existente a qual queira adicionar a integra��o da Twitch utilizando a biblioteca StreamingClientLibrary.

#### *O primeiro passo e arrastar ou colar somente as DLLs StreamingClient.Base.dll e a Twitch.Base.dll que foram compilada anteriormente para a pasta Assets do seu projeto Unity*

#### *Dentro da pasta Assets crie uma nova pasta chamada Integrations*

#### *Agora dentro desta pasta, adicione um novo script chamado 'TwitchIntegration'. Esta sera uma classe de integra��o com a Twitch, portanto ser� consumida por outras controllers, portanto n�o ir� herdar a MonoBehaviour*

### **Codificando a TwitchIntegration.cs**

Inicialmente, limpe a classe gerada pela unity, removendo a heran�a de Game Object, remova os metodos Start() e Update() e por uma boa pr�tica, envolva a classe dentro de um namespace com o mesmo nome da pasta em que esta contida: 'Integrations' e por fim torne a classe Estatica, visto que havera apenas uma unica conex�o com a twitch por vez em todo ciclo de vida do seu projeto.

```
namespace Integrations
{
    public static class TwitchIntegration
    {
  
    }
}
```

Agora vamos declarar todas as constantes, que ser�o as propriedades imut�veis ap�s a compila��o do projeto. Para isto, tenha em m�os aquelas chaves que a Twitch gerou no console de desenvolvimento que foi solicitado que fosse armazenado em lugar seguro. Estes valores ser�o preenchidos nas constantes ClientID e ClientSecret.
Quanto a constante SuccessResponseHTMLBody voc� pode depois customizar ela ao seu gosto, ela ser� o conte�do que ir� aparecer no navegador assim que a autoriza��o com a twitch for bem sucedida.
Quanto a constante SuccesRedirectURL, veja que ela � a mesma URL que voc� cadastrou no console de desenvolvimento da Twitch. Ou seja, se for personalizar esta URL, voc� tamb�m precisa configurar a respectiva URL neste painel.
J� a constante AuthorizationScopes ela dita sobre que permiss�es de acesso estamos requisitando para o usu�rio da twitch que est� se conectando no momento. Dependendo dos servi�os consumidos, precisamos solicitar escopos diferentes, mas por ora partiremos apenas de ler dados do usu�rio logado.

```
const string ClientID = "qbuvvgggk6tev6bonpvswxk8e4e6uc";
const string ClientSecret = "bo85843vcdgrd78o4x7w4qj446ojyj";
const string SuccessResponseHTMLBody = "<html><body>Voc� esta logado. Pode fechar esta p�gina agora.<body></html>";
const string TwitchAuthBaseURL = "https://id.twitch.tv/oauth2/authorize";
const string SuccesRedirectURL = "http://localhost:8919";
const string AuthorizationScopes = "user:read:email";
```

Depois das constantes, vamos declarar todas a propriedades somente leitura, que ser�o preenchidas somente pela pr�pria classe de integra��o com a Twitch, mas que voc� podera consumir fora dela:

- AuthorizationCode: O Token de autentica��o OAuth que sera utilizado nas chamadas da API da Twitch.
- IsAuthorizing: Indica se o processo esta em fase de autoriza��o
- IsAuthorized: Indica se o processo de autoriza��o foi conclu�do e que j� temos o acesso de consulta da API liberado
- IsConnecting: Indica se o processo de conex�o com a Twitch est� em andamento
- IsConnected: Indica se o processo de conex�o foi executado com sucesso
- CurrentUser: Objeto que contem dados do usu�rio da twitch que logou na aplica��o
- CurrentChannel: Objeto que contem informa��es do canal do usu�rio da twitch que logou na a plica��o

```
public static string AuthorizationCode { get; private set; }
public static bool IsAuthorizing { get; private set; }
public static bool IsAuthorized { get; private set; }
public static bool IsConnecting { get; private set; }
public static bool IsConnected { get; private set; }
public static UserModel CurrentUser { get; private set; }
public static ChannelInformationModel CurrentChannel { get; private set;}
```

Como as conex�es s�o assincronas e orientada a eventos, vamos declarar os delegates e seus respectivos eventos

```
public delegate void TwitchConnectionHandler();
public delegate void TwitchConnectionSuccessHandler(UserModel currentUser, ChannelInformationModel currentChannel);

public static event TwitchConnectionHandler OnTwitchConnecting;
public static event TwitchConnectionHandler OnTwitchDisconnected;
public static event TwitchConnectionHandler OnTwitchConnectFail;
public static event TwitchConnectionSuccessHandler OnTwitchConnected;
```

Temos a seguir a declara��o dos objetos privados, que manter�o a conex�o e a autentica��o

���
private static OAuthTokenModel oauthModel;
private static TwitchConnection connection;
���

Precisamos de um metodo agora, que receba uma conex�o twitch, e controle se ela � nova. Se afirmativo, cria uma copia do seu token para futura requisi��o e dispara o evento de conex�o

```
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
```

O metodo acima, ser� utilizado pelo seguinte metodo, que ir� controlar as conex�es existentes, renovando seu token de acesso quando preciso.

``````
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
``````

Agora vamos criar o m�todo que ir� receber um *HttpListenerContext* e processar o seu conte�do para obter o token retornado pela Twitch e ent�o renderizar o HTML da constante *SuccessResponseHTMLBody* no navegador que foi aberto.

```
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
```

E por fim, o principal metodo, que ser� utilizado pela sua aplica��o para solicitar uma autoriza��o e conex�o com a Twitch:

```
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
        //Iniciando um Servidor HTTP para receber a resposta da Twitch
        httpListener.Start();

        ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = twitchAuthURL, UseShellExecute = true };
        //Abrindo a p�gina de autentica��o da Twitch no navegador padr�o
        //para que o usu�rio da twitch possa autorizar a aplica��o
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
```

## **Criando a controller de conex�o da Twitch na Unity**

#### *Dentro da pasta Assets crie uma nova pasta chamada Controllers*

#### *Dentro da pasta Controllers, crie uma nova pasta chamada TwitchAPI*

#### *Agora dentro desta pasta, adicione um novo script chamado 'TwitchController'. Esta sera um MonoBehaviour que ir� se conectar com a twitch*

### **Codificando a TwitchController.cs**

Inicialmente, limpe a classe gerada pela unity, removendo os metodos Start() e Update().
Por boa pr�tica, envolva a classe com o namespace seguindo a estrutura de pastas criadas:

``````
using UnityEngine;

namespace Controllers.TwitchAPI
{
}
``````

Agora adicione os metodos OnEnable() e OnDisable(), que servir�o para a registrar os eventos assim que o objeto vor iniciado, e desfazer o registro dos eventos assim que a classe for disposta ou desabilitada.

```
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
```

Agora precisamos criar os metodos registrados nos eventos acima

```
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
```

E por fim, vamos criar o metodo que ira chamar a conex�o com a Twitch.

```
public void Connect()
    => Integrations.TwitchIntegration.Authorize();
```

Este metodo poder� ser chamado por qualquer outro objeto ou ate mesmo por um evento de algum GameObject como um clique de um bot�o que e o que veremos na pr�xima etapa.

## **Criando um bot�o na cena**

Estes passos serao realizados na Unity

#### *Na sua cena, crie um Canvas, e dentro deste canvas, um Bot�o*

#### *No canvas, adicione como componente o script TwitchController*

#### *No bot�o, Adicione um evento OnClic, movendo o Canvas como objeto, e selecionando o metodo TwitchController.Connect*

![](assets/20220310_220759_image.png)

![](assets/20220310_220811_image.png)

## **Finalizando**
Pronto, antes de rodar sua aplica��o, confirme se voc� alterou as constantes ClientID e ClientSecret para as que voc� criou no console de desenvolvimento da Twitch. Ap�s isto, ao rodar a aplica��o, voc� ser� redirecionado para uma p�gina de autoriza��o da twitch, e confirmando o acesso, a aplica��o ir� se conectar e exibir os dados do seu usu�rio no console da unity.

![](assets/20220310_221112_image.png)
