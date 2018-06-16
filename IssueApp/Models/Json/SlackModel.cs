using Newtonsoft.Json;
using System.Collections.Generic;

namespace IssueApp.Models.Json
{
    public class SlackModel<T>
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("response_type")]
        public string Response_type { get; set; }

        [JsonIgnore]
        [JsonProperty("attachments")]
        public List<AttachmentModel<T>> Attachments { get; set; }

        public SlackModel()
        {
            Response_type = "in_channel";
        }
    }
}