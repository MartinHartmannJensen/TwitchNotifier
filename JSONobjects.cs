using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ArethruNotifier
{
    public class StreamsInfo
    {
        [JsonProperty("streams")]
        public List<StreamsObj> Streams { get; set; }

        private bool isSucces = true;
        public bool IsSucces { get { return isSucces; } set { isSucces = value; } }

        public string DebugMessage { get; set; }
    }

    public class StreamsObj
    {
        [JsonProperty("_id")]
        public string ID { get; set; }

        [JsonProperty("game")]
        public string Game { get; set; }

        [JsonProperty("viewers")]
        public string Viewers { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        public string CreatedAt_short { get {
                return CreatedAt.ToShortTimeString();
            } }

        [JsonProperty("channel")]
        public Channel Channel { get; set; }
    }

    public class Channel
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("display_name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        public string Url_Profile { get { return Url + "/profile"; } }

        [JsonProperty("logo")]
        public string Logo { get; set; }
    }

    public class Token
    {
        [JsonProperty("access_token")]
        public string Access_Token { get; set; }
    }

    public class Follows
    {
        [JsonProperty("follows")]
        public List<Follow> List { get; set; }

        [JsonProperty("_total")]
        public int Total { get; set; }

        private bool isSucces = true;
        public bool IsSucces { get { return isSucces; } set { isSucces = value; } }

        public string DebugMessage { get; set; }
    }

    public class Follow
    {
        [JsonProperty("channel")]
        public Channel Channel { get; set; }
    }

    public class User
    {
        [JsonProperty("display_name")]
        public string Name { get; set; }
    }
}
