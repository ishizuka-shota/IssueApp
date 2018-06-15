using Microsoft.WindowsAzure.Storage.Table;

namespace IssueApp.Models.Entity
{
    /// <summary>
    /// チャンネル用Entity
    /// チャンネル1つにつき、リポジトリ1つ
    /// </summary>
    public class ChannelIdEntity : TableEntity
    {
        public ChannelIdEntity(string channelId, string repository)
        {
            PartitionKey = channelId;
            RowKey = "repository";
            Repository = repository;
        }

        public ChannelIdEntity() { }

        public string Repository { get; set; }
        public string BotToken { get; set; }
    }
}