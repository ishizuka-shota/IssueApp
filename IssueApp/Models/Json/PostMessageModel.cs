using Newtonsoft.Json;
using System.Collections.Generic;

namespace IssueApp.Models.Json
{
    public class PostMessageModel
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("response_type")]
        public string Response_type { get; set; }

        [JsonProperty("attachments")]
        public List<Attachment> Attachments { get; set; }

        [JsonProperty("unfurl_links")]
        public bool Unfurl_links { get; set; }

        /// <summary>
        /// Attachment定義
        /// </summary>
        public class Attachment
        {
            [JsonProperty("fallback")]
            public string Fallback { get; set; }

            [JsonProperty("attachment_type")]
            public string Attachment_type { get; set; }

            [JsonProperty("callback_id")]
            public string Callback_id { get; set; }

            [JsonProperty("actions")]
            public List<Action> Actions { get; set; }

            /// <summary>
            /// Action定義
            /// </summary>
            public class Action
            {
                [JsonProperty("name")]
                public string Name { get; set; }

                [JsonProperty("text")]
                public string Text { get; set; }

                [JsonProperty("type")]
                public string Type { get; set; }

                [JsonProperty("url")]
                public string Url { get; set; }

                [JsonProperty("style")]
                public string Style { get; set; }

                [JsonProperty("options")]
                public List<Option> Options { get; set; }

                /// <summary>
                /// Option定義
                /// </summary>
                public class Option
                {
                    [JsonProperty("text")]
                    public string Text { get; set; }

                    [JsonProperty("value")]
                    public string Value { get; set; }

                    public Option(string text, string value)
                    {
                        Text = text;
                        Value = value;
                    }

                }
            }
        }
    }
}