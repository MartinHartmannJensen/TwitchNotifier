using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        static string userToken = Settings.Default.UserToken;

        //'authURL + parameters' creates the full url for authorizing a user profile for the clientID
        static string authURL = "https://api.twitch.tv/kraken/oauth2/authorize";
        static string responseType = "?response_type=token"; //'code' for server/'token' for app
        static string redirectURI = "http://localhost:4515/oauth2/authorize";
        static string scopes = "user_follows_edit";
        static string parameters = string.Format("{0}&client_id={1}&redirect_uri={2}&scope={3}", responseType, clientID, redirectURI, scopes);

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

            return "something went wrong cap'n          " + response.StatusCode.ToString();
        }

        public static StreamsInfo GetLiveStreams()
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

            var failData = new StreamsInfo();
            failData.isSucces = false;

            return failData;
        }

        public static void OpenBrowserAuthenticate()
        {
            System.Diagnostics.Process.Start(authURL + parameters);
        }
    }
}
