using Newtonsoft.Json;
using System.Collections.Generic;

namespace IssueApp.Models.Request
{
    public class SlackRequest
    {
        public class Active<T>
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("token")]
            public string Token { get; set; }

            [JsonProperty("action_ts")]
            public string Action_ts { get; set; }

            [JsonProperty("team")]
            public Team Team { get; set; }

            [JsonProperty("user")]
            public User User { get; set; }

            [JsonProperty("channel")]
            public Channel Channel { get; set; }

            [JsonProperty("submission")]
            public T Submission { get; set; }

            [JsonProperty("callback_id")]
            public string Callback_id { get; set; }

            [JsonProperty("response_url")]
            public string Response_url { get; set; }
        }
  
    }

    public class Team
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("domain")]
        public string Domain { get; set; }
    }

    public class User
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class Channel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class IssueData
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }
    }

    public class RepositoryData
    {
        [JsonProperty("username")]
        public string UserName { get; set; }

        [JsonProperty("repository")]
        public string Repository { get; set; }
    }
}