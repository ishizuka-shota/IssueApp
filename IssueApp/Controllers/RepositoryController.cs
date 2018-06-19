using IssueApp.AzureTableStorage;
using IssueApp.Models.Entity;
using IssueApp.Models.Json;
using IssueApp.Slack;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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


        #region リポジトリ操作エンドポイント
        /// <summary>
        /// リポジトリ操作エンドポイント
        /// </summary>
        /// <param name="request"></param>
        [HttpPost]
        [Route("api/repository")]
        public async Task Operation(HttpRequestMessage request)
        {
            // ===========================
            // リクエストの取得・整形
            // ===========================
            string content = await request.Content.ReadAsStringAsync();
            NameValueCollection data = HttpUtility.ParseQueryString(content);

            // 引数の値
            string method = data["text"];

            // 引数の値で処理を分ける
            switch (method)
            {
                // リポジトリ登録
                case "set":
                    {
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
                        break;
                    }
                // 登録リポジトリ照会
                case "get":
                    {
                        // PartitionKeyがチャンネルIDのEntityを取得するクエリ
                        TableQuery<ChannelIdEntity> query = new TableQuery<ChannelIdEntity>()
                            .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, data["channel_id"]));

                        string text = string.Empty;
                        // クエリ実行結果で要素がひとつでもあるかどうか
                        if (StorageOperation.GetTableIfNotExistsCreate("channel").ExecuteQuery(query).Any())
                        {
                            string repository = StorageOperation.GetTableIfNotExistsCreate("channel").ExecuteQuery(query).First().Repository;
                            text = "登録リポジトリのURLを照会します" + Environment.NewLine + "https://github.com/" + repository;
                        }
                        else
                        {
                            text = "登録リポジトリは存在しません";
                        }

                        SlackModel<ButtonActionModel> model = new SlackModel<ButtonActionModel>()
                        {
                            Channel = data["channel_id"],
                            Text = text,
                            Response_type = "ephemeral"
                        };

                        HttpResponseMessage response = await slackApi.ExecutePostApiAsJson(model, data["response_url"], data["team_id"]);
                        break;
                    }
                // それ以外
                default:
                    {
                        SlackModel<ButtonActionModel> model = new SlackModel<ButtonActionModel>()
                        {
                            Channel = data["channel_id"],
                            Text = "引数を入力してください",
                            Response_type = "ephemeral"
                        };

                        HttpResponseMessage response = await slackApi.ExecutePostApiAsJson(model, data["response_url"], data["team_id"]);

                        break;
                    }
            }
            
        }
        #endregion
    }
}
