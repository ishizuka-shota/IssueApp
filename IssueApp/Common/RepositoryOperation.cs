using IssueApp.AzureTableStorage;
using IssueApp.Models.Entity;
using IssueApp.Models.Json;
using IssueApp.Slack;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IssueApp.Common
{
    /// <summary>
    /// リポジトリ操作用クラス
    /// </summary>
    public static class RepositoryOperation
    {
        #region リポジトリ照会
        /// <summary>
        /// リポジトリ照会
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="responseUrl"></param>
        /// <param name="teamId"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static async Task GetRepository(string channelId, string responseUrl, string teamId, Func<string, Task> func)
        {
            // PartitionKeyがチャンネルIDのEntityを取得するクエリ
            var query = new TableQuery<ChannelIdEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, channelId));

            // クエリ実行
            var entityList = StorageOperation.GetTableIfNotExistsCreate("channel").ExecuteQuery(query);

            // クエリ実行結果で要素がひとつでもあるかどうか
            var channelIdEntities = entityList.ToList();
            if (channelIdEntities.Any())
            {
                await func(channelIdEntities.First().Repository);
            }
            else
            {
                var model = new PostMessageModel()
                {
                    Channel = channelId,
                    Text = "登録リポジトリが存在しません。",
                    Response_type = "ephemeral"
                };

                var slackApi = new SlackApi();

                await slackApi.ExecutePostApiAsJson(model, responseUrl, teamId);
            }
        }
        #endregion
    }
}