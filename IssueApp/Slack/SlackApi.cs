using IssueApp.AzureTableStorage;
using IssueApp.Models.Entity;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace IssueApp.Slack
{
    public class SlackApi
    {
        #region Json送信Api実行(post)
        /// <summary>
        /// Api実行(post)
        /// </summary>
        /// <param name="slackModel"></param>
        /// <param name="apiUri"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> ExecutePostApiAsJson(object model, string apiUri, string teamId)
        {
            using (HttpClient client = CreateHeaderAsJson(teamId))
            {
                // モデルからJson作成s
                string json = JsonConvert.SerializeObject(model, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

                // リクエスト作成
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, apiUri)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };

                // API実行
                HttpResponseMessage response =  await client.SendAsync(httpRequest);

                return response;
            }
        }
        #endregion

        #region Json送信SlackApi用ヘッダー作成
        /// <summary>
        /// Json送信SlackApi用ヘッダー作成
        /// </summary>
        /// <returns></returns>
        public HttpClient CreateHeaderAsJson(string teamId)
        {
            // PartitionKeyがチームIDのEntityを取得するクエリ
            TableQuery<TeamIdEntity> query = new TableQuery<TeamIdEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, teamId));

            // クエリ実行結果で要素がひとつでもあるかどうか
            if (StorageOperation.GetTableIfNotExistsCreate("team").ExecuteQuery(query).Any())
            {
                // クライアント作成
                HttpClient client = new HttpClient();

                // ヘッダー情報挿入
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", StorageOperation.GetTableIfNotExistsCreate("team").ExecuteQuery(query).First().Token);

                return client;
            }
            else
            {
                throw new Exception();
            }
        }
        #endregion
    }
}