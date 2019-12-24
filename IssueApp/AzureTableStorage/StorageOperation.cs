using IssueApp.Models.Entity;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;

namespace IssueApp.AzureTableStorage
{
    public class StorageOperation
    {
        #region Azure Storage Table接続
        /// <summary>
        /// Azure Storage Table接続
        /// </summary>
        public static CloudTable GetTableIfNotExistsCreate(string tableName)
        {
            // Azure Storageアカウント認証
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["TableStorage"].ConnectionString);

            // アカウントからテーブルクライアントを取得
            var tableClient = storageAccount.CreateCloudTableClient();

            // テーブル名が空だったら
            if (string.IsNullOrEmpty(tableName))
            {
                // PartitionKeyがチャンネルIDのEntityを取得するクエリ
                var query = new TableQuery<ChannelIdEntity>()
                    .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "GitHubDialog.channelId"));

                // クエリ実行結果で要素がひとつでもあるかどうか
                if (GetTableIfNotExistsCreate("repository").ExecuteQuery(query).Any())
                {
                    // クエリ実行結果の一番最初を持ってくる
                    var entity = GetTableIfNotExistsCreate("repository").ExecuteQuery(query).First();

                    // repository名から半角英数字を抜き出し、先頭にTをつけたものをテーブル名とする
                    var regular = new Regex(@"[^0-9a-zA-Z]");
                    tableName = "T" + regular.Replace(entity.Repository.Split('/')[1], "");
                }
            }

            // テーブルクライアントからテーブルを選択
            var table = tableClient.GetTableReference(tableName);

            // テーブルがなかったらテーブル作成
            table.CreateIfNotExists();

            return table;

        }
        #endregion
    }
}