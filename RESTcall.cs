using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace ArethruTwitchNotifier
{
    static class RESTcall
    {
        //Appspecific and usersensitive data
        static string clientID = MyClientIDfile.clientID; //Just replace this with your Twitch Client ID given to you
        static string userToken = Properties.Settings.Default.UserToken;

        //'authURL + parameters' creates the full url for authorizing a user profile for the clientID
        static string authURL = "https://api.twitch.tv/kraken/oauth2/authorize";
        static string responseType = "?response_type=token"; //'code' for server/'token' for app
        static string redirectURI = "http://localhost:4515/oauth2/authorize";
        static string scopes = "user_follows_edit";
        static string parameters = string.Format("{0}&client_id={1}&redirect_uri={2}&scope={3}", responseType, clientID, redirectURI, scopes);


        //New authentication
        static string qAuthURL = @"https://api.twitch.tv/kraken/oauth2/authorize" +
                                "?response_type=code" + 
                                "&client_id=" + MyClientIDfile.clientID +
                                "&redirect_uri=" + redirectURI + 
                                "&scope=user_follows_edit" + 
                                "&state=DKDKDK123";

        static string qPostBody = @"client_id=" + MyClientIDfile.clientID +
            "&client_secret=" + MyClientIDfile.clientSecret +
            "&grant_type=authorization_code" +
            "&redirect_uri=" + redirectURI +
            "&code=" +
            "&state=DKDKDK123";

        

        public static string AuthURL { get { return authURL + parameters; } }

        public static string GetLiveStreamsFullString()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.twitch.tv/kraken/streams/followed");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync("?oauth_token=" + userToken).Result;

            if (response.IsSuccessStatusCode)
            {
                var data = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                return data.ToString();
            }

            return response.StatusCode.ToString();
        }

        public static StreamsInfo GetLiveStreams()
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://api.twitch.tv/kraken/streams/followed");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("?oauth_token=" + userToken).Result;

                if (response.IsSuccessStatusCode)
                {
                    var data = JsonConvert.DeserializeObject<StreamsInfo>(response.Content.ReadAsStringAsync().Result);
                    data.isSucces = true;

                    return data;
                }
                return new StreamsInfo() { isSucces = false };
            }
            catch (System.ArgumentNullException)
            {
            }
            catch (System.Net.WebException)
            {
            }
            catch (System.Net.Http.HttpRequestException)
            {
            }
            catch (System.Net.Sockets.SocketException)
            {
            }
            catch (System.AggregateException)
            {
            }
            return new StreamsInfo() { isSucces = false };
        }

        public static void OpenBrowserAuthenticate()
        {
            System.Diagnostics.Process.Start(qAuthURL);
        }

        public static string ListenForResponse()
        {
            //Listen for redirect
            HttpListener listen = new HttpListener();
            listen.Prefixes.Add(redirectURI + @"/");
            listen.Start();
            HttpListenerContext context = listen.GetContext();
            listen.Stop();

            if (context == null)
                return "error getting token";

            string queryCode = context.Request.QueryString.Get(0).ToString();


            //POST to their server

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.twitch.tv");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var pairs = new List<KeyValuePair<string, string>>();
            pairs.Add(new KeyValuePair<string, string>("client_id", MyClientIDfile.clientID));
            pairs.Add(new KeyValuePair<string, string>("client_secret", MyClientIDfile.clientSecret));
            pairs.Add(new KeyValuePair<string, string>("grant_type", "authorization_code"));
            pairs.Add(new KeyValuePair<string, string>("redirect_uri", redirectURI));
            pairs.Add(new KeyValuePair<string, string>("code", queryCode));
            pairs.Add(new KeyValuePair<string, string>("state", "DKDKDK123"));

            var cont = new System.Net.Http.FormUrlEncodedContent(pairs);

            var response = client.PostAsync(@"/kraken/oauth2/token", cont).Result;

            if (response.IsSuccessStatusCode)
            {
                var data = JsonConvert.DeserializeObject<Token>(response.Content.ReadAsStringAsync().Result);

                return data.Access_Token;
            }

            return "error getting token";
        }
    }
}
