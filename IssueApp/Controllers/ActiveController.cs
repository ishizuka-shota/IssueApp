using IssueApp.Models.Json;
using IssueApp.Slack;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace IssueApp.Controllers
{
    public class ActiveController : ApiController
    {
        #region 【変数】SlackApi実行用変数
        /// <summary>
        /// SlackApi実行用変数
        /// </summary>
        private SlackApi slackApi = new SlackApi();
        #endregion


        #region アクティブ操作エンドポイント
        /// <summary>
        /// アクティブ操作エンドポイント
        /// </summary>
        /// <param name="request"></param>
        [HttpPost]
        [Route("api/active")]
        public async Task Post(HttpRequestMessage request)
        {
            string content = await request.Content.ReadAsStringAsync();
            NameValueCollection json = HttpUtility.ParseQueryString(content);

            dynamic data = JsonConvert.DeserializeObject(json["payload"]);

            string callback = data["callback_id"];

            // ===========================
            // callback_idに応じて処理
            // ===========================
            switch(callback)
            {
                case "setrepository":
                    {
                        SetRepository(data);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

           
        }
        #endregion

        public async Task SetRepository(dynamic data)
        {
            SlackModel<ButtonActionModel> model = new SlackModel<ButtonActionModel>()
            {
                Channel = data["channel"]["id"],
                Text = "https://github.com/" + data["submission"]["username"] + "/" + data["submission"]["repository"],
                Unfurl_links = true
            };

            HttpResponseMessage response = await slackApi.ExecutePostApiAsJson(model, "https://slack.com/api/chat.postMessage");
        }
    }
}