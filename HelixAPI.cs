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

        public static async Task<Users> GetUser(string name) {
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
    }
}
