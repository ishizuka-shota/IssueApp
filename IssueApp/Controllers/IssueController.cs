using IssueApp.AzureTableStorage;
using IssueApp.Common;
using IssueApp.GitHub;
using IssueApp.Models.Entity;
using IssueApp.Models.Json;
using IssueApp.Slack;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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

        #region エラーハンドラー
        /// <summary>
        /// エラーハンドラー
        /// </summary>
        private ErrorHandler errorHandler = new ErrorHandler();
        #endregion


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
           
            // ===========================
            // リクエストの取得・整形
            // ===========================
            string content = await request.Content.ReadAsStringAsync();
            NameValueCollection data = HttpUtility.ParseQueryString(content);

            // =============================
            // GitHubアクセストークンの設定
            // =============================
            GitHubApi.SetCredential(data["user_id"]);

            // =============================
            // 登録リポジトリ取得
            // =============================
            TableQuery<ChannelIdEntity> query = new TableQuery<ChannelIdEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, data["channel_id"]));

            // クエリ実行
            var entityList = StorageOperation.GetTableIfNotExistsCreate("channel").ExecuteQuery(query);

            // クエリ実行結果で要素がひとつでもあるかどうか
            if (entityList.Any())
            {
                string repository = entityList.First().Repository;

                List<string> labelNameList = null;

                // GitHub認証エラーハンドリング
                await errorHandler.AuthorizationExceptionHandler(data["channel_id"], data["response_url"], data["team_id"], async () =>
                {
                    // クライアントを用いてリポジトリ名からIssueのラベルを取得
                    var labelList = await GitHubApi.client.Issue.Labels.GetAllForRepository(repository.Split('/')[0], repository.Split('/')[1]);

                    // ラベル変数リストを文字列リストに変換
                    labelNameList = labelList.ToList().ConvertAll(x => x.Name);
                });
                
                DialogModel model = new DialogModel()
                {
                    Trigger_id = data["trigger_id"],
                    Dialog = new Dialog()
                    {
                        Callback_id = "createissue",
                        Title = "Issue登録",
                        Submit_label = "登録",
                        Elements = new List<Element>
                    {
                        new Element()
                        {
                            Type = "select",
                            Label = "ラベル",
                            Name = "label",
                            Options = labelNameList.ConvertAll(x => new Option(x, x))

                        },
                        new Element()
                        {
                            Type = "text",
                            Label = "タイトル",
                            Name = "title"
                        },
                        new Element()
                        {
                            Type = "textarea",
                            Label = "本文",
                            Name = "body"
                        }
                    }
                    }
                };

                HttpResponseMessage response = await slackApi.ExecutePostApiAsJson(model, "https://slack.com/api/dialog.open", data["team_id"]);
            }
            else
            {
                SlackModel<ButtonActionModel> model = new SlackModel<ButtonActionModel>()
                {
                    Channel = data["channel_id"],
                    Text = "登録リポジトリは存在しません",
                    Response_type = "ephemeral"
                };

                HttpResponseMessage response = await slackApi.ExecutePostApiAsJson(model, data["response_url"], data["team_id"]);
            }
                     
        }
        #endregion

    }
}
