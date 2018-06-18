using IssueApp.AzureTableStorage;
using IssueApp.Models.Entity;
using IssueApp.Models.Json;
using IssueApp.Slack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
                        SetRepository(data);
                        break;
                    }
                case "url":
                    {
                        RedirectUrl(data);
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
        public async Task SetRepository(dynamic data)
        {
            string channelId = data["channel"]["id"];
            string channelName = data["channel"]["name"];
            string repository = data["submission"]["username"] + "/" + data["submission"]["repository"];

            // 保存するトークンを入れたentityを作成
            ChannelIdEntity entity = new ChannelIdEntity(channelId, channelName, repository);

            // Entityがなければ挿入、あれば更新する
            var insertResult = entityOperation_ChannelId.InsertOrUpdateEntityResult(entity, "channel");

            string text = string.Empty;

            // 結果があるかどうか
            if (insertResult != null)
            {
                text = "リポジトリの登録に成功しました。" + Environment.NewLine + "https://github.com/" + data["submission"]["username"] + "/" + data["submission"]["repository"];
            }
            else
            {
                text = "リポジトリの登録に失敗しました";
            }

            SlackModel<ButtonActionModel> model = new SlackModel<ButtonActionModel>()
            {
                Channel = data["channel"]["id"],
                Text = text,
                Response_type = "in_channel",
                Unfurl_links = true
            };

            string teamId = data["team"]["id"];

            HttpResponseMessage response = await slackApi.ExecutePostApiAsJson(model, "https://slack.com/api/chat.postMessage", teamId);
        }
        #endregion

        public void RedirectUrl(dynamic data)
        {
            string url = data["actions"][0]["value"];

            Redirect(url);
        }
    }
}