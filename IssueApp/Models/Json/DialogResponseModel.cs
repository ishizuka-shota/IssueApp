using Newtonsoft.Json;
using System.Collections.Generic;

namespace IssueApp.Models.Json
{
    public class DialogResponseModel
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("actions")]
        public List<Action> Actions { get; set; }

        [JsonProperty("callback_id")]
        public string Callback_id { get; set; }

        [JsonProperty("team")]
        public Team Team { get; set; }

        [JsonProperty("channel")]
        public Channel Channel { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("action_ts")]
        public string Action_ts { get; set; }

        [JsonProperty("message_ts")]
        public string Message_ts { get; set; }

        [JsonProperty("attachment_id")]
        public string Attachment_id { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("original_message")]
        public string Original_message { get; set; }

        [JsonProperty("response_url")]
        public string Response_url { get; set; }

        [JsonProperty("trigger_id")]
        public string Trigger_id { get; set; }
    }

    public class Action
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class Team
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("domain")]
        public string Domain { get; set; }
    }

    public class Channel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class User
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}