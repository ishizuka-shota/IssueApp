using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IssueApp.Models.Json
{
    public class OptionModel
    {
        public string Text { get; set; }

        public string Value { get; set; }

        public OptionModel(string text, string value)
        {
            Text = text;
            Value = value;
        }
    }
}