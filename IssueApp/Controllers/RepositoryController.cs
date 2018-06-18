using IssueApp.Models.Json;
using IssueApp.Slack;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace IssueApp.Controllers
{
    public class RepositoryController : ApiController
    {
        #region 【変数】SlackApi実行用変数
        /// <summary>
        /// SlackApi実行用変数
        /// </summary>
        private SlackApi slackApi = new SlackApi();
        #endregion


        #region リポジトリ登録エンドポイント
        /// <summary>
        /// リポジトリ登録エンドポイント
        /// </summary>
        /// <param name="request"></param>
        [HttpPost]
        [Route("api/repository")]
        public async Task Set(HttpRequestMessage request)
        {
            string content = await request.Content.ReadAsStringAsync();
            NameValueCollection data = HttpUtility.ParseQueryString(content);

            DialogModel model = new DialogModel()
            {
                Trigger_id = data["trigger_id"],
                Dialog = new Dialog()
                {
                    Callback_id = "setrepository",
                    Title = "リポジトリ登録",
                    Submit_label = "登録",
                    Elements = new List<Element>
                    {
                        new Element()
                        {
                            Type = "text",
                            Label = "ユーザ名",
                            Name = "username"
                        },
                        new Element()
                        {
                            Type = "text",
                            Label = "リポジトリ名",
                            Name = "repository"
                        }
                    }
                }
            };

            HttpResponseMessage response = await slackApi.ExecutePostApiAsJson(model, "https://slack.com/api/dialog.open", data["team_id"]);
        }
        #endregion
    }
}
