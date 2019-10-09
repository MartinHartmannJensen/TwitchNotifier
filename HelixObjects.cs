using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Text;

namespace ArethruNotifier.Helix {
    //public class StreamsInfo {
    //    public List<User> users;
    //    public List<Stream> streams;


    //}

    public class Users {
        bool isOk = false;
        public bool IsOk { get; set; }

        [JsonProperty("data")]
        public List<User> User { get; set; }
    }

    public class User {
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class Streams {
        bool isOk = false;
        public bool IsOk { get; set; }

        [JsonProperty("data")]
        public List<Stream> Stream { get; set; }

        public DateTime CreationTime { get; private set; }

        public Streams TimeSorted() {
            if (Stream == null) {
                return this;
            }
            this.Stream.OrderByDescending(x => x.CreatedAt);
            return this;
        }

        public Streams() {
            CreationTime = DateTime.Now;
        }

        public string GenerateUserIds() {
            var returns = new StringBuilder();
            foreach (var item in Stream) {
                returns.Append("id=" + item.GameId + "&");
            }
            returns.Remove(returns.Length - 1, 1);

            return returns.ToString();
        }
    }

    public class Stream {
        [JsonProperty("user_name")]
        public string Channel { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        private string game = "";
        public string Game { get { return game; } set { game = value; } }

        public bool IsLive {
            get {
                if (Type.Equals("live")) {
                    return true;
                }
                return false;
            }
        }

        [JsonProperty("started_at")]
        public string CreatedAt { get; set; }

        public string CreatedAt_Short { get {
                return DateTime.Parse(CreatedAt).ToShortTimeString();
            } }

        [JsonProperty("viewer_count")]
        public string Viewers { get; set; }

        [JsonProperty("game_id")]
        public string GameId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }

    public class Follows {
        bool isOk = false;
        public bool IsOk { get; set; }

        [JsonProperty("data")]
        public List<Follow> Follow { get; set; }

        public string GenerateUserIds() {
            var returns = new StringBuilder();
            foreach (var item in Follow) {
                returns.Append("user_id=" + item.Id + "&");
            }
            returns.Remove(returns.Length - 1, 1);

            return returns.ToString();
        }
    }

    public class Follow {
        [JsonProperty("to_id")]
        public string Id { get; set; }

        [JsonProperty("to_name")]
        public string Name { get; set; }
    }

    // todo
    // get game object

    public class Games {
        bool isOk = false;
        public bool IsOk { get; set; }

        [JsonProperty("data")]
        public List<Game> Game { get; set; }
    }

    public class Game {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
