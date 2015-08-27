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
    public class StreamsInfo
    {
        [JsonProperty("streams")]
        public List<StreamsObj> Streams { get; set; }

        public bool isSucces { get; set; }
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

        [JsonProperty("channel")]
        public StreamsObjChannel Channel { get; set; }
    }

    public class StreamsObjChannel
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("display_name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class Token
    {
        [JsonProperty("access_token")]
        public string Access_Token { get; set; }
    }
}
