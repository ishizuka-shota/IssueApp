using Newtonsoft.Json;
using System.Collections.Generic;
using static IssueApp.Models.Json.PostMessageModel;

namespace IssueApp.Models.Json
{
    public class UpdateModel
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("ts")]
        public string Ts { get; set; }

        [JsonProperty("attachments")]
        public List<Attachment> Attachments { get; set; }
    }
}