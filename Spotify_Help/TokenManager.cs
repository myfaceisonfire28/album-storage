using System;
using System.Net;
using System.Threading.Tasks;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;

namespace Music.Spotify_Help
{
    public class TokenManager
    {
        private readonly static string ClientID = "NOT GIVING";
        private readonly static string ClientSecret = "NOT GIVING";
        static readonly HttpListener listener = new HttpListener();
        private static string Token = " ";
        private static string RefreshToken = " ";
        public static SpotifyClient SpotClient;
        public static void GetToken()
        {
            //TODO MAKE SURE EXPIRED TOKENS ARE CHECKED ON AND STUFF PLUS MAKE SURE ONLY THING THAT CAN USE SPOTIFY SHIT IS THE TOKEN/SPOTIFY HELPER RAHH RAHH
            var loginRequest = new LoginRequest(
                new Uri("http://127.0.0.1:5543/callback"),
                ClientID,
                LoginRequest.ResponseType.Code
            )
            {
                Scope = new[] {
                    Scopes.UserModifyPlaybackState,
                    Scopes.UserReadCurrentlyPlaying,
                    Scopes.UserReadPlaybackState,
                    Scopes.PlaylistModifyPrivate,
                    Scopes.PlaylistModifyPublic,
                }
            };
            var Uri = loginRequest.ToUri();
            listener.Prefixes.Add("http://127.0.0.1:5543/");
            listener.Start();
            BrowserUtil.Open(Uri);

            bool RunServer = true;
            string? code;
            while (RunServer == true)
            {
                HttpListenerContext ctx = listener.GetContext();
                HttpListenerRequest req = ctx.Request;
                if ((req.HttpMethod == "GET") && (req.Url.AbsolutePath == "/callback"))
                {
                    code = req.Url.AbsoluteUri;
                    code = code.Split('=')[1];
                    RunServer = false;
                    GetCallback(code).Wait();
                }
            }
            listener.Close();

        }

        ///Gets the response from the web server
        private static async Task GetCallback(string code)//Gets the token using the code and ClientID
        {
            var initialResponse = await new OAuthClient().RequestToken(
            new AuthorizationCodeTokenRequest(ClientID, ClientSecret, code, new Uri("http://127.0.0.1:5543/callback"))
            );
            Token = initialResponse.AccessToken;
            RefreshToken = initialResponse.RefreshToken;
            SpotClient = new SpotifyClient(Token);
        }

        ///Refreshes the token
        public static async Task RefreshTheToken()
        {
            var newResponse = await new OAuthClient().RequestToken(
            new AuthorizationCodeRefreshRequest("ClientId", "ClientSecret", RefreshToken)
            );
            Token = newResponse.AccessToken;
            RefreshToken = newResponse.RefreshToken;
            SpotClient = new SpotifyClient(Token);
        }
    }
}