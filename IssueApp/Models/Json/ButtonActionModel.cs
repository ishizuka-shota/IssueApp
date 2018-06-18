using Newtonsoft.Json;

namespace IssueApp.Models.Json
{
    public class ButtonActionModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("options")]
        public string Options { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}