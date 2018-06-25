using Newtonsoft.Json;

namespace IssueApp.Models.Json.Parts
{
    /// <summary>
    /// Option定義
    /// </summary>
    public class Option
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }
    }
}