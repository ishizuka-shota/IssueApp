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
        /// <param name="model"></param>
        /// <param name="apiUri"></param>
        /// <param name="teamId"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> ExecutePostApiAsJson(object model, string apiUri, string teamId)
        {
            using (var client = CreateHeaderAsJson(teamId))
            {
                // モデルからJson作成
                var json = JsonConvert.SerializeObject(model, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

                // リクエスト作成
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, apiUri)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };

                // API実行
                var response =  await client.SendAsync(httpRequest);

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
            var query = new TableQuery<TeamIdEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, teamId));

            var entityList = StorageOperation.GetTableIfNotExistsCreate("team").ExecuteQuery(query);

            // クエリ実行結果で要素がひとつでもあるかどうか
            var teamIdEntities = entityList.ToList();
            if (teamIdEntities.Any())
            {
                // クライアント作成
                var client = new HttpClient();

                // ヘッダー情報挿入
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", teamIdEntities.First().Token);

                return client;
            }

            throw new Exception();
        }
        #endregion
    }
}