using System;
using System.Configuration;
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
        public async Task<HttpResponseMessage> ExecutePostApiAsJson(object model, string apiUri)
        {
            using (HttpClient client = CreateHeaderAsJson())
            {
                // モデルからJson作成s
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(model);

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
        public HttpClient CreateHeaderAsJson()
        {
            // クライアント作成
            HttpClient client = new HttpClient();

            // ヘッダー情報挿入
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigurationManager.AppSettings["botToken"]);

            return client;
        }
        #endregion
    }
}