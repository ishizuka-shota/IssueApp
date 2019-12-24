using IssueApp.AzureTableStorage;
using IssueApp.Models.Entity;
using Microsoft.WindowsAzure.Storage.Table;
using Octokit;
using System.Linq;

namespace IssueApp.GitHub
{
    public class GitHubApi
    {
        #region GitHubクライアント
        /// <summary>
        /// GitHubクライアント
        /// </summary>
        public static GitHubClient Client = new GitHubClient(new ProductHeaderValue("IssueApp"));
        #endregion


        #region GitHubアクセストークンセット
        /// <summary>
        /// GitHubアクセストークンセット
        /// </summary>
        /// <param name="slackUserId"></param>
        public static void SetCredential(string slackUserId)
        {
            // PartitionKeyがSlackユーザIDのEntityを取得するクエリ
            TableQuery<UserEntity> query = new TableQuery<UserEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, slackUserId));

            // クエリ実行
            var entityList = StorageOperation.GetTableIfNotExistsCreate("user").ExecuteQuery(query);

            // クエリ実行結果で要素がひとつでもあるかどうか
            var userEntities = entityList.ToList();
            Client.Credentials = userEntities.Any() ? new Credentials(userEntities.First().AccessToken) : new Credentials("aaaaaaaaaaaaaa");
        }
        #endregion

    }
}