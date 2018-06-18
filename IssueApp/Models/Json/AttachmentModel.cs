using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IssueApp.Models.Json
{
    public class AttachmentModel<T>
    {
        [JsonProperty("fallback")]
        public string Fallback { get; set; }

        [JsonProperty("attachment_type")]
        public string Attachment_type { get; set; }

        [JsonProperty("callback_id")]
        public string Callback_id { get; set; }

        [JsonProperty("actions")]
        public List<T> Actions { get; set; }
    }
}