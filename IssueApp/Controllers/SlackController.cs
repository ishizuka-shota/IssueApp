using IssueApp.Models.Entity;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static IssueApp.Common.CommonUtility;

namespace IssueApp.Controllers
{
    public class SlackController : ApiController
    {
        #region Slack Oauth認証
        /// <summary>
        /// Slack Oauth認証
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/slack/oauth")]
        public async Task<HttpResponseMessage> SlackOauth(string code, string state)
        {
            // ==============================
            // アクセストークン発行
            // ==============================
            HttpClient client = new HttpClient();

            var query = HttpUtility.ParseQueryString(string.Empty);
            query["client_id"] = ConfigurationManager.AppSettings["slackapp_client_id"];
            query["client_secret"] = ConfigurationManager.AppSettings["slackapp_client_secret"];
            query["code"] = code;

            UriBuilder builder = new UriBuilder(new Uri("https://slack.com/api/oauth.access"))
            {
                Query = query.ToString()
            };

            HttpResponseMessage response = await client.GetAsync(builder.Uri.ToString());

            // ================================
            // アクセストークンをStorageに保存
            // ================================
            string content = await response.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(content);

            string teamId = data["team_id"];
            string teamName = data["team_name"];
            string token = data["bot"]["bot_access_token"];

            // 保存するトークンを入れたentityを作成
            TeamIdEntity entity = new TeamIdEntity(teamId, teamName, token);

            // Entityがなければ挿入、あれば更新する
            var insertResult = entityOperation_TeamId.InsertOrUpdateEntityResult(entity, "team");

            // 結果があるかどうか
            if (insertResult != null)
            {
                // あったら認証成功
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("SlackのOauth認証に成功しました")
                };
            }
            else
            {
                // なければ認証失敗
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("SlackのOauth認証に失敗しました")
                };
            }
        }
        #endregion
    }
}
