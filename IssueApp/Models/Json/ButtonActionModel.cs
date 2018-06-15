using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IssueApp.Models.Json
{
    public class ButtonActionModel
    {
        public string Name { get; set; }

        public string Text { get; set; }

        public string Type { get; set; }

        public string Options { get; set; }

        public ButtonActionModel(String value)
        {
            Type = "button";
            Options = value;
        }
    }
}