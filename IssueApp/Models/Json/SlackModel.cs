using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace IssueApp.Models.Json
{
    public class PostMessageModel<T>
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("response_type")]
        public string Response_type { get; set; }

        [JsonProperty("attachments")]
        public List<AttachmentModel<T>> Attachments { get; set; }

        [JsonProperty("unfurl_links")]
        public bool Unfurl_links { get; set; }
    }
}