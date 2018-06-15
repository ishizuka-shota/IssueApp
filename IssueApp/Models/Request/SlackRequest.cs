﻿namespace IssueApp.Models.Request
{
    public class SlackRequest
    {
        public string Token { get; set; }
        public string Team_id { get; set; }
        public string Team_domain { get; set; }
        public string Channel_id { get; set; }
        public string Channel_name { get; set; }
        public string User_id { get; set; }
        public string User_name { get; set; }
        public string Command { get; set; }
        public string Text { get; set; }
        public string Response_url { get; set; }
        public string Trigger_id { get; set; }
    }
}