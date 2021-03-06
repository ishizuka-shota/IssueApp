﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace IssueApp.Models.Json
{
    public class DialogModel
    {
        [JsonProperty("trigger_id")]
        public string Trigger_id { get; set; }

        [JsonProperty("dialog")]
        public Dialog Dialog { get; set; }

    }

    public class Dialog
    {
        [JsonProperty("callback_id")]
        public string Callback_id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("submit_label")]
        public string Submit_label { get; set; }

        /// <summary>
        /// キャンセルボタンを追加するかどうか(デフォは追加する)
        /// </summary>
        [JsonProperty("notify_on_cancel")]
        [DefaultValue(true)]
        public bool Notify_on_cancel { get; set; }

        [JsonProperty("elements")]
        public List<Element> Elements { get; set; }

        public class Element
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("label")]
            public string Label { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("options")]
            public List<Option> Options { get; set; }

            public class Option
            {
                [JsonProperty("label")]
                public string Label { get; set; }

                [JsonProperty("value")]
                public string Value { get; set; }

                public Option(string label, string value)
                {
                    Label = label;
                    Value = value;
                }

            }
        }
    }
}