using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SGDBMetadata
{
    public class SearchModel
    {
        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("release_date")]
        public long? release_date { get; set; }

        [JsonProperty("types")]
        public List<string> types { get; set; } = new List<string>();

        [JsonProperty("verified")]
        public bool verified { get; set; }
    }
}
