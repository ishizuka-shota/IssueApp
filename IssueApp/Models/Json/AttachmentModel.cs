using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IssueApp.Models.Json
{
    public class AttachmentModel<T>
    {
        public string Fallback { get; set; }

        public string Attachment_type { get; set; }

        public string Callback_id { get; set; }

        public List<T> Actions { get; set; }

        public AttachmentModel()
        {
            Fallback = "This is slack api";
            Attachment_type = "default";
        }
    }
}