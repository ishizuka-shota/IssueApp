using IssueApp.AzureTableStorage;
using IssueApp.Models.Entity;
using IssueApp.Models.Json;
using IssueApp.Slack;
using Microsoft.WindowsAzure.Storage.Table;
using Octokit;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Security;

namespace IssueApp.Controllers
{
    public class GitHubController : ApiController
    {
        #region 【変数】GitHubクライアント変数
        /// <summary>
        /// GitHubクライアント変数
        /// </summary>
        private GitHubClient githubClient = new GitHubClient(new ProductHeaderValue("IssueApp"));
        #endregion

        #region 【変数】SlackApi実行用変数
        /// <summary>
        /// SlackApi実行用変数
        /// </summary>
        private SlackApi slackApi = new SlackApi();
        #endregion

        #region 【変数】method間値渡し用変数
        /// <summary>
        /// 値渡し用変数
        /// </summary>
        private static NameValueCollection UserData;
        #endregion


        #region 【GET】GitHubアクセストークン発行
        /// <summary>
        /// 【GET】GitHubアクセストークン発行
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/github/token")]
        public async Task<RedirectResult> IssueToken(string code, string state)
        {
            // Oauth認証時に格納したユーザデータを取得
            NameValueCollection data = UserData;

            // ===========================
            // GitHubアクセストークン発行
            // ===========================
            var request = new OauthTokenRequest(ConfigurationManager.AppSettings["client_id"], ConfigurationManager.AppSettings["client_secret"], code);

            var token = await githubClient.Oauth.CreateAccessToken(request);

            // ============================================
            // slackユーザとGitHubアクセストークンの紐付け
            // ============================================
            EntityOperation<UserEntity> entityOperation_User = new EntityOperation<UserEntity>();

            //作成or更新を行うユーザエンティティ作成
            UserEntity entity = new UserEntity(data["user_id"], data["user_name"], token.AccessToken);

            //エンティティを操作変数を用いて作成or更新
            TableResult result = entityOperation_User.InsertOrUpdateEntityResult(entity, "user");

            return Redirect("https://" + data["team_domain"] + ".slack.com/messages");

        }
        #endregion

        #region 【POST】GitHub Oauth認証
        /// <summary>
        /// 【POST】GitHub Oauth認証
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/github/oauth")]
        public async Task LoginGitHub(HttpRequestMessage request)
        {
            string content = await request.Content.ReadAsStringAsync();
            NameValueCollection data = HttpUtility.ParseQueryString(content);

            // アクセストークン保存の為のユーザデータを格納
            UserData = data;

            // ===========================
            // GitHub OauthURL取得
            // ===========================
            var csrf = Membership.GeneratePassword(20, 0);

            var oauthRequest = new OauthLoginRequest(ConfigurationManager.AppSettings["client_id"])
            {
                Scopes = { "repo", "user" },
                State = csrf
            };

            string url = githubClient.Oauth.GetGitHubLoginUrl(oauthRequest).ToString();

            // ==============================
            // Oauth用リダイレクトボタン作成
            // ==============================
            SlackModel<ButtonActionModel> model = new SlackModel<ButtonActionModel>()
            {
                Channel = data["channel_id"],
                Text = "GitHubへログインしてください",
                Response_type = "ephemeral",
                Attachments = new List<AttachmentModel<ButtonActionModel>>()
                {
                    new AttachmentModel<ButtonActionModel>()
                    {
                        Fallback = "The GitHub Oauth URL",
                        Actions = new List<ButtonActionModel>()
                        {
                            new ButtonActionModel()
                            {
                                Type = "button",
                                Name = "github_oauth_url",
                                Text = "ログイン",
                                Url = url,
                                Style = "primary"
                            }
                        }
                    }
                }
            };

            HttpResponseMessage response = await slackApi.ExecutePostApiAsJson(model, data["response_url"], data["team_id"]);
        }
        #endregion
    }
}
