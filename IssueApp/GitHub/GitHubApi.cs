using IssueApp.AzureTableStorage;
using IssueApp.Models.Entity;
using Microsoft.WindowsAzure.Storage.Table;
using Octokit;
using System.Linq;

namespace IssueApp.GitHub
{
    public class GitHubApi
    {
        private static GitHubClient client = new GitHubClient(new ProductHeaderValue("IssueApp"));

        #region GitHubアクセストークンセット
        /// <summary>
        /// GitHubアクセストークンセット
        /// </summary>
        /// <param name="slack_userId"></param>
        public void SetCredential(string slack_userId)
        {
            // PartitionKeyがSlackユーザIDのEntityを取得するクエリ
            TableQuery<UserEntity> query = new TableQuery<UserEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, slack_userId));

            // クエリ実行
            var entityList = StorageOperation.GetTableIfNotExistsCreate("user").ExecuteQuery(query);

            // クエリ実行結果で要素がひとつでもあるかどうか
            if (entityList.Any())
            {
                client.Credentials = new Credentials(entityList.First().AccessToken);
            }
            else
            {
                // クレデンシャル情報に適当な値を入れ、認証エラーが起きるようにする
                client.Credentials = new Credentials("aaaaaaaaaaaaaa");
            }
        }
        #endregion

    }
}