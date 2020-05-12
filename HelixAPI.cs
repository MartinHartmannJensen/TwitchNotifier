using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ArethruNotifier.Helix {
    public static class HelixAPI {
        static readonly HttpClient client = new HttpClient();
        static string clientID = MyClientID.clientID;
        static string clientSecret = MyClientID.clientSecret;
        static string redirectURI = "http://localhost:4515/oauth2/authorize/arethrunotifier";
        public static string AuthURL = @"https://id.twitch.tv/oauth2/authorize" +
                                "?client_id=" + clientID +
                                "&redirect_uri=" + redirectURI +
                                "&response_type=code" +  
                                "&force_verify=true";

        static string htmlResponseSuccess = "<HTML><BODY style='text-align:center'>"
            + "<h3>Success!</h3><br>"
            + "Settings saved. Restarting application."
            + "</BODY></HTML>";

        // Get user by Bearer Token
        public static async Task<Users> GetUser(string token) {
            try {
                var request = new HttpRequestMessage(HttpMethod.Get,
                    "https://api.twitch.tv/helix/users");
                request.Headers.Add("Client-ID", clientID);
                request.Headers.Add("Authorization", "Bearer " + token);
                var response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode) {
                    return new Users();
                }

                var data = JsonConvert.DeserializeObject<Users>(await response.Content.ReadAsStringAsync());
                data.IsOk = true;
                return data;
            }
            catch (System.Net.Http.HttpRequestException e) {
                var ee = e;
            }

            return new Users();
        }

        // Get user by name
        public static async Task<Users> GetUserBy(string name) {
            try {
                var request = new HttpRequestMessage(HttpMethod.Get,
                    "https://api.twitch.tv/helix/users?login=" + name);
                request.Headers.Add("Client-ID", clientID);
                var response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode) {
                    return new Users();
                }

                var data = JsonConvert.DeserializeObject<Users>(await response.Content.ReadAsStringAsync());
                data.IsOk = true;
                return data;
            }
            catch (System.Net.Http.HttpRequestException e) {
                var ee = e;
            }

            return new Users();
        }

        public static async Task<Follows> GetFollows(string id, int counts = 100) {
            try {
                var request = new HttpRequestMessage(HttpMethod.Get,
                    @"https://api.twitch.tv/helix/users/follows?first=" + counts + "&from_id=" + id);
                request.Headers.Add("Client-ID", clientID);
                request.Headers.Add("Authorization", "Bearer " + ConfigMgnr.I.Token);
                var response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode) {
                    return new Follows();
                }

                var data = JsonConvert.DeserializeObject<Follows>(await response.Content.ReadAsStringAsync());
                data.IsOk = true;
                return data;
            }
            catch (System.Net.Http.HttpRequestException) { }

            return new Follows();
        }

        public static async Task<Streams> GetStreams(string ids) {
            try {
                var request = new HttpRequestMessage(HttpMethod.Get,
                    @"https://api.twitch.tv/helix/streams?" + ids);
                request.Headers.Add("Client-ID", clientID);
                request.Headers.Add("Authorization", "Bearer " + ConfigMgnr.I.Token);
                var response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode) {
                    return new Streams();
                }

                var data = JsonConvert.DeserializeObject<Streams>(await response.Content.ReadAsStringAsync());
                data.IsOk = true;
                return data;
            }
            catch (System.Net.Http.HttpRequestException) { }

            return new Streams();
        }

        public static async Task<Games> GetGames(string ids) {
            try {
                var request = new HttpRequestMessage(HttpMethod.Get,
                    @"https://api.twitch.tv/helix/games?" + ids);
                request.Headers.Add("Client-ID", clientID);
                request.Headers.Add("Authorization", "Bearer " + ConfigMgnr.I.Token);
                var response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode) {
                    return new Games();
                }

                var data = JsonConvert.DeserializeObject<Games>(await response.Content.ReadAsStringAsync());
                data.IsOk = true;
                return data;
            }
            catch (System.Net.Http.HttpRequestException) { }

            return new Games();
        }

        public async static Task<string> ListenForResponse() {
            //Listen for redirect
            string returnString = "null";
            string htmlMsg = htmlResponseSuccess;
            HttpListener listen = new HttpListener();
            listen.Prefixes.Add(redirectURI + @"/");
            listen.Start();
            HttpListenerContext context = await listen.GetContextAsync();
            HttpListenerResponse lResponse = context.Response;
            string queryCode = context.Request.QueryString.Get(0).ToString();


            //POST to their server
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://id.twitch.tv");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var pairs = new List<KeyValuePair<string, string>>();
            pairs.Add(new KeyValuePair<string, string>("client_id", clientID));
            pairs.Add(new KeyValuePair<string, string>("client_secret", clientSecret));
            pairs.Add(new KeyValuePair<string, string>("code", queryCode));
            pairs.Add(new KeyValuePair<string, string>("grant_type", "authorization_code"));
            pairs.Add(new KeyValuePair<string, string>("redirect_uri", redirectURI));
            var cont = new FormUrlEncodedContent(pairs);

            var response = client.PostAsync(@"/oauth2/token", cont).Result;

            // Response Handling and HTTPlistener response message
            if (response.IsSuccessStatusCode) {
                var data = JsonConvert.DeserializeObject<Token>(response.Content.ReadAsStringAsync().Result);
                returnString = data.Access_Token;
            }
            else {
                htmlMsg = "Error in getting authentication response. Please try again.";
            }

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(htmlMsg);
            lResponse.ContentLength64 = buffer.Length;
            System.IO.Stream output = lResponse.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();

            listen.Stop();
            return returnString;
        }
    }
}
