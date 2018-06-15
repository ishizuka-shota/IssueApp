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
        public async void Post(HttpRequestMessage request)
        {
            string content = await request.Content.ReadAsStringAsync();
            NameValueCollection json = HttpUtility.ParseQueryString(content);

            dynamic data = JsonConvert.DeserializeObject(json["payload"]);

            SlackModel<ButtonActionModel> model = new SlackModel<ButtonActionModel>()
            {
                Text = data["submission"]["username"] + data["submission"]["repository"],
                Response_type = "in_channel",
                Replace_original = false,
                Delete_original = true
            };

            await slackApi.ExecutePostApiAsJson(model, "https://slack.com/api/chat.postMessage");
        }
        #endregion
    }
}