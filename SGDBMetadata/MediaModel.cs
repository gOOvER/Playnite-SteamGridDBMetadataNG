using Newtonsoft.Json;
using System.Collections.Generic;

namespace SGDBMetadata
{
    public class MediaModel
    {
        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("score")]
        public int score { get; set; }

        [JsonProperty("style")]
        public string style { get; set; }

        [JsonProperty("width")]
        public int width { get; set; }

        [JsonProperty("height")]
        public int height { get; set; }

        [JsonProperty("nsfw")]
        public bool nsfw { get; set; }

        [JsonProperty("humor")]
        public bool humor { get; set; }

        [JsonProperty("epilepsy")]
        public bool epilepsy { get; set; }

        [JsonProperty("notes")]
        public string notes { get; set; }

        [JsonProperty("mime")]
        public string mime { get; set; }

        [JsonProperty("language")]
        public string language { get; set; }

        [JsonProperty("url")]
        public string url { get; set; }

        [JsonProperty("thumb")]
        public string thumb { get; set; }

        [JsonProperty("tags")]
        public List<string> tags { get; set; } = new List<string>();

        [JsonProperty("author")]
        public AuthorSchema author { get; set; }
    }
}
