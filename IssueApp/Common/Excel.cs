using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Octokit;

namespace IssueApp.Common
{
    public class Excel
    {
        public void exportIssue(List<Issue> issues)
        {
            var excelApp = new Microsoft.Office.Interop.Excel.Application();
        }
    }
}