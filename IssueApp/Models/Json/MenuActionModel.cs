using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IssueApp.Models.Json
{
    public class MenuActionModel
    {
        public string Name { get; set; }

        public string Text { get; set; }

        public string Type { get; set; }

        public List<OptionModel> Options { get; set; }

        public MenuActionModel(List<string> list)
        {
            Type = "select";
            Options = list.ConvertAll(x => new OptionModel(x, x));
        }
    }
}