﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ArethruNotifier.Kraken {
    public static class APIcalls {
        static string clientID = MyClientID.clientID;
        static string clientSecret = MyClientID.clientSecret;
        static string userToken = ConfigMgnr.I.UserToken;
        static string redirectURI = "http://localhost:4515/oauth2/authorize/arethrunotifier";
        static string scopes = "user_read";

        public static string AuthURL = @"https://api.twitch.tv/kraken/oauth2/authorize" +
                                "?response_type=code" +
                                "&client_id=" + clientID +
                                "&redirect_uri=" + redirectURI +
                                "&scope=" + scopes +
                                "&state=somestate123&force_verify=true";

        static string htmlResponseSuccess = "<HTML><BODY style='text-align:center'>"
            + "<h3>Success!</h3><br>"
            + "Settings saved. Restarting application."
            +"</BODY></HTML>";

        public static string GetStreamHelix() {
            try {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://api.twitch.tv/helix/streams");
                client.DefaultRequestHeaders.Add("Client-ID", clientID);
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("?first=20&user_login=arethru").Result;

                if (!response.IsSuccessStatusCode) {
                    return response.ToString();
                }
                return response.Content.ReadAsStringAsync().Result;
            }
            catch (System.ArgumentNullException) { }
            catch (System.Net.WebException) { }
            catch (System.Net.Http.HttpRequestException) { }
            catch (System.Net.Sockets.SocketException) { }
            catch (System.AggregateException) { }
            return "Error";
        }

        public static Streams GetLiveStreams() {
            try {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://api.twitch.tv/kraken/streams/followed");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("?oauth_token=" + userToken).Result;

                if (response.IsSuccessStatusCode) {
                    var data = JsonConvert.DeserializeObject<Streams>(response.Content.ReadAsStringAsync().Result);

                    return data;
                }
                return new Streams() { IsSuccess = false, DebugMessage = "GET /streams/followed   -   error: " + response.StatusCode.ToString() };
            }
            catch (System.ArgumentNullException) { }
            catch (System.Net.WebException) { }
            catch (System.Net.Http.HttpRequestException) { }
            catch (System.Net.Sockets.SocketException) { }
            catch (System.AggregateException) { }
            return new Streams() { IsSuccess = false };
        }

        public static async Task<Streams> GetLiveStreamsTask() {
            try {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://api.twitch.tv/kraken/streams/followed");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("?oauth_token=" + userToken);

                if (response.IsSuccessStatusCode) {
                    var data = JsonConvert.DeserializeObject<Streams>(await response.Content.ReadAsStringAsync());

                    return data;
                }
                return new Streams() { IsSuccess = false, DebugMessage = "GET /streams/followed   -   error: " + response.StatusCode.ToString() };
            }
            catch (System.ArgumentNullException) { }
            catch (System.Net.WebException) { }
            catch (System.Net.Http.HttpRequestException) { }
            catch (System.Net.Sockets.SocketException) { }
            catch (System.AggregateException) { }
            return new Streams() { IsSuccess = false };
        }

        public static Follows GetFollowedChannels() {
            try {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://api.twitch.tv");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("/kraken/user?oauth_token=" + userToken).Result;

                if (!response.IsSuccessStatusCode)
                    return new Follows() { IsSucces = false, DebugMessage = "GET /user   -   error: " + response.StatusCode.ToString() };

                var thisuser = JsonConvert.DeserializeObject<User>(response.Content.ReadAsStringAsync().Result);

                HttpResponseMessage response2 = client.GetAsync("/kraken/users/" + thisuser.Name + "/follows/channels?limit=100&oauth_token=" + userToken).Result;

                if (response2.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Follows>(response2.Content.ReadAsStringAsync().Result);

                return new Follows() { IsSucces = false, DebugMessage = "GET /users/:user/follows/channels   -   error: " + response2.StatusCode.ToString() };
            }
            catch (System.ArgumentNullException) { }
            catch (System.Net.WebException) { }
            catch (System.Net.Http.HttpRequestException) { }
            catch (System.Net.Sockets.SocketException) { }
            catch (System.AggregateException) { }
            return new Follows() { IsSucces = false };
        }

        public static void OpenBrowserAuthenticate() {
            System.Diagnostics.Process.Start(AuthURL);
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
            client.BaseAddress = new Uri("https://api.twitch.tv");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var pairs = new List<KeyValuePair<string, string>>();
            pairs.Add(new KeyValuePair<string, string>("client_id", clientID));
            pairs.Add(new KeyValuePair<string, string>("client_secret", clientSecret));
            pairs.Add(new KeyValuePair<string, string>("grant_type", "authorization_code"));
            pairs.Add(new KeyValuePair<string, string>("redirect_uri", redirectURI));
            pairs.Add(new KeyValuePair<string, string>("code", queryCode));
            pairs.Add(new KeyValuePair<string, string>("state", "DKDKDK123"));

            var cont = new FormUrlEncodedContent(pairs);

            var response = client.PostAsync(@"/kraken/oauth2/token", cont).Result;


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