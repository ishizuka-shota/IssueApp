using Microsoft.WindowsAzure.Storage.Table;

namespace IssueApp.Models.Entity
{
    /// <summary>
    /// チャンネル用Entity
    /// チャンネル1つにつき、リポジトリ1つ
    /// </summary>
    public class ChannelIdEntity : TableEntity
    {
        public ChannelIdEntity(string channelId, string channelName, string repository)
        {
            PartitionKey = channelId;
            RowKey = channelName;
            Repository = repository;
        }

        public ChannelIdEntity() { }

        public string Repository { get; set; }
    }
}