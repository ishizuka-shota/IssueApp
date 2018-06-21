using IssueApp.AzureTableStorage;
using IssueApp.GitHub;
using IssueApp.Models.Entity;
using IssueApp.Models.Json;
using IssueApp.Models.Request;
using IssueApp.Slack;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Octokit;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
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

        #region 【変数】Entity操作用変数(ChannelId)
        /// <summary>
        /// Entity操作用変数(ChannelId)
        /// </summary>
        private static EntityOperation<ChannelIdEntity> entityOperation_ChannelId = new EntityOperation<ChannelIdEntity>();
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
            ChannelIdEntity entity = new ChannelIdEntity(data.Channel.Id, data.Channel.Name, data.Submission.Repository);

            // Entityがなければ挿入、あれば更新する
            var insertResult = entityOperation_ChannelId.InsertOrUpdateEntityResult(entity, "channel");

            string text = string.Empty;

            // 結果があるかどうか
            if (insertResult != null)
            {
                text = "リポジトリの登録に成功しました。" + Environment.NewLine + "https://github.com/" + data.Submission.UserName + "/" + data.Submission.Repository;
            }
            else
            {
                text = "リポジトリの登録に失敗しました";
            }

            SlackModel<ButtonActionModel> model = new SlackModel<ButtonActionModel>()
            {
                Channel = data.Channel.Id,
                Text = text,
                Response_type = "in_channel",
                Unfurl_links = true
            };

            HttpResponseMessage response = await slackApi.ExecutePostApiAsJson(model, "https://slack.com/api/chat.postMessage", data.Team.Id);
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
            NewIssue newIssue = new NewIssue(data.Submission.Title)
            {
                Body = data.Submission.Body
            };

            newIssue.Labels.Add(data.Submission.Label);
            
            // =============================
            // GitHubアクセストークンの設定
            // =============================
            GitHubApi.SetCredential(data.User.Id);

            // PartitionKeyがチャンネルIDのEntityを取得するクエリ
            TableQuery<ChannelIdEntity> query = new TableQuery<ChannelIdEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, data.Channel.Id));

            // クエリ実行
            var entityList = StorageOperation.GetTableIfNotExistsCreate("channel").ExecuteQuery(query);

            SlackModel<ButtonActionModel> model = new SlackModel<ButtonActionModel>();

            if (entityList.Any())
            {
                string repository = entityList.First().Repository;
                var issue = await GitHubApi.client.Issue.Create(repository.Split('/')[0], repository.Split('/')[1], newIssue);

                model = new SlackModel<ButtonActionModel>()
                {
                    Channel = data.Channel.Id,
                    Text = "Issueが登録されました" + Environment.NewLine + "https://github.com/" + repository + "/issues/" + issue.Number,
                    Response_type = "in_channel",
                    Unfurl_links = true
                };
            }
            else
            {
                model = new SlackModel<ButtonActionModel>()
                {
                    Channel = data.Channel.Id,
                    Text = "リポジトリが登録されていません",
                    Response_type = "ephemeral"
                };
            }

            HttpResponseMessage response = await slackApi.ExecutePostApiAsJson(model, data.Response_url, data.Team.Id);

        }
        #endregion
    }
}