using IssueApp.GitHub;
using IssueApp.Models.Entity;
using IssueApp.Models.Json;
using IssueApp.Models.Request;
using Newtonsoft.Json;
using Octokit;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using static IssueApp.Common.CommonUtility;
using static IssueApp.Common.ErrorHandler;
using static IssueApp.Common.RepositoryOperation;


namespace IssueApp.Controllers
{
    public class ActiveController : ApiController
    {
        #region アクティブ操作エンドポイント
        /// <summary>
        /// アクティブ操作エンドポイント
        /// </summary>
        /// <param name="request"></param>
        [HttpPost]
        [Route("api/active")]
        public async Task Post(HttpRequestMessage request)
        {
            // ===========================
            // リクエストの取得・整形
            // ===========================
            NameValueCollection json = await GetBody(request);

            dynamic data = JsonConvert.DeserializeObject(json["payload"]);

            string callback = data["callback_id"];

            // ===========================
            // callback_idに応じて処理
            // ===========================
            switch(callback)
            {
                case "setrepository":
                    {
                        var req = JsonConvert.DeserializeObject<SlackRequest.Active<RepositoryData>>(json["payload"]);
                        await SetRepository(req);
                        break;
                    }
                case "createissue":
                    {
                        var req = JsonConvert.DeserializeObject<SlackRequest.Active<IssueData>>(json["payload"]);
                        await CreateIssue(req);
                        break;
                    }
                default:
                    {
                        request.CreateResponse(HttpStatusCode.OK, "操作が定義されていません。");
                        break;
                    }
            }
        }
        #endregion


        #region リポジトリ登録
        /// <summary>
        /// リポジトリ登録
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task SetRepository(SlackRequest.Active<RepositoryData> data)
        {
            // 保存するトークンを入れたentityを作成
            var entity = new ChannelIdEntity(data.Channel.Id, data.Channel.Name, data.Submission.UserName + "/" + data.Submission.Repository);

            // Entityがなければ挿入、あれば更新する
            var insertResult = EntityOperationChannelId.InsertOrUpdateEntityResult(entity, "channel");

            string text;

            // 結果があるかどうか
            if (insertResult != null)
            {
                text = "リポジトリの登録に成功しました。" + Environment.NewLine + "https://github.com/" + data.Submission.UserName + "/" + data.Submission.Repository;
            }
            else
            {
                text = "リポジトリの登録に失敗しました";
            }

            var model = new PostMessageModel()
            {
                Channel = data.Channel.Id,
                Text = text,
                Response_type = "in_channel",
                Unfurl_links = true
            };

            await SlackApi.ExecutePostApiAsJson(model, "https://slack.com/api/chat.postMessage", data.Team.Id);
        }
        #endregion

        #region Issue作成
        /// <summary>
        /// Issue作成
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task CreateIssue(SlackRequest.Active<IssueData> data)
        {
            // =============================
            // Issueオブジェクト作成
            // =============================
            var newIssue = new NewIssue(data.Submission.Title)
            {
                Body = data.Submission.Body
            };

            newIssue.Labels.Add(data.Submission.Label);
            
            // =============================
            // GitHubアクセストークンの設定
            // =============================
            GitHubApi.SetCredential(data.User.Id);

            // =============================
            // 登録リポジトリ取得
            // =============================
            await GetRepository(data.Channel.Id, data.Response_url, data.Team.Id, async repository =>
            {
                // GitHub認証エラーハンドリング
                await AuthorizationExceptionHandler(data.Channel.Id, data.Response_url, data.Team.Id, async () =>
                {
                    // =============================
                    // Issue作成
                    // =============================
                    var issue = await GitHubApi.Client.Issue.Create(repository.Split('/')[0], repository.Split('/')[1], newIssue);

                    PostMessageModel model = new PostMessageModel()
                    {
                        Channel = data.Channel.Id,
                        Text = "Issueが登録されました" + Environment.NewLine + "https://github.com/" + repository + "/issues/" + issue.Number,
                        Response_type = "in_channel",
                        Unfurl_links = true
                    };

                    await SlackApi.ExecutePostApiAsJson(model, data.Response_url, data.Team.Id);
                });
            });
        }
        #endregion
    }
}