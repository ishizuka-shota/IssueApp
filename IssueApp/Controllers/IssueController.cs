﻿using IssueApp.GitHub;
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
    public class IssueController : ApiController
    {
        #region 【変数】SlackApi実行用変数
        /// <summary>
        /// SlackApi実行用変数
        /// </summary>
        private SlackApi slackApi = new SlackApi();
        #endregion

        #region 【変数】GitHubApi実行用変数
        /// <summary>
        /// GitHubApi実行用変数
        /// </summary>
        private GitHubApi githubApi = new GitHubApi();
        #endregion

        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        #region Issue作成エンドポイント
        /// <summary>
        /// Issue作成エンドポイント
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/issue")]
        public async Task Create(HttpRequestMessage request)
        {
            string content = await request.Content.ReadAsStringAsync();
            NameValueCollection data = HttpUtility.ParseQueryString(content);

            githubApi.SetCredential(data["user_id"]);

            DialogModel model = new DialogModel()
            {
                Trigger_id = data["trigger_id"],
                Dialog = new Dialog()
                {
                    Callback_id = "setrepository",
                    Title = "Issue登録",
                    Submit_label = "登録",
                    Elements = new List<Element>
                    {
                        new Element()
                        {
                            Type = "select",
                            Label = "タイトル",
                            Name = "title"
                        },
                        new Element()
                        {
                            Type = "text",
                            Label = "タイトル",
                            Name = "title"
                        }
                    }
                }
            };

            HttpResponseMessage response = await slackApi.ExecutePostApiAsJson(model, "https://slack.com/api/dialog.open", data["team_id"]);

        }
        #endregion

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
