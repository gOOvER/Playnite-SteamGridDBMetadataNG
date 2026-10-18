using Newtonsoft.Json;
using System.Collections.Generic;

namespace SGDBMetadata
{
    public class ResponseModel<T>
    {
        [JsonProperty("success")]
        public bool success { get; set; }

        [JsonProperty("data")]
        public List<T> data { get; set; } = new List<T>();

        [JsonProperty("errors")]
        public List<string> errors { get; set; } = new List<string>();

        [JsonProperty("page")]
        public int? page { get; set; }

        [JsonProperty("total")]
        public int? total { get; set; }

        [JsonProperty("limit")]
        public int? limit { get; set; }
    }

    public class SingleResponseModel<T>
    {
        [JsonProperty("success")]
        public bool success { get; set; }

        [JsonProperty("data")]
        public T data { get; set; }

        [JsonProperty("errors")]
        public List<string> errors { get; set; } = new List<string>();
    }
}
