using IssueApp.Models.Entity;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;

namespace IssueApp.AzureTableStorage
{
    public class StorageOperation
    {
        #region 【変数】Entity操作用変数(ChannelId)
        /// <summary>
        /// Entity操作用変数(CHannelId)
        /// </summary>
        private static EntityOperation<ChannelIdEntity> entityOperation_ChannelId = new EntityOperation<ChannelIdEntity>();
        #endregion


        #region Azure Storage Table接続
        /// <summary>
        /// Azure Storage Table接続
        /// </summary>
        public static CloudTable GetTableIfNotExistsCreate(string tableName)
        {
            // Azure Storageアカウント認証
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["TableStorage"].ConnectionString);

            // アカウントからテーブルクライアントを取得
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // テーブル名が空だったら
            if (String.IsNullOrEmpty(tableName))
            {
                // PartitionKeyがチャンネルIDのEntityを取得するクエリ
                TableQuery<ChannelIdEntity> query = new TableQuery<ChannelIdEntity>()
                    .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "GitHubDialog.channelId"));

                // クエリ実行結果で要素がひとつでもあるかどうか
                if (GetTableIfNotExistsCreate("repository").ExecuteQuery(query).Any())
                {
                    // クエリ実行結果の一番最初を持ってくる
                    ChannelIdEntity entity = GetTableIfNotExistsCreate("repository").ExecuteQuery(query).First();

                    // repository名から半角英数字を抜き出し、先頭にTをつけたものをテーブル名とする
                    var regular = new Regex(@"[^0-9a-zA-Z]");
                    tableName = "T" + regular.Replace(entity.Repository.Split('/')[1], "");
                }
            }

            // テーブルクライアントからテーブルを選択
            CloudTable table = tableClient.GetTableReference(tableName);

            // テーブルがなかったらテーブル作成
            table.CreateIfNotExists();

            return table;

        }
        #endregion
    }
}