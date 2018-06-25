﻿using IssueApp.Models.Json;
using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using static IssueApp.Common.CommonUtility;
using static IssueApp.Common.RepositoryOperation;
using static IssueApp.Models.ModelOperation;

namespace IssueApp.Controllers
{
    public class RepositoryController : ApiController
    {
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
            NameValueCollection data = await GetBody(request);

            // 引数の値
            string method = data["text"];

            // ===========================
            // 引数の値で処理を分ける
            // ===========================
            switch (method)
            {
                // ===========================
                // リポジトリ登録
                // ===========================
                case "set":
                    {
                        // ====================================
                        // リポジトリ登録用ダイアログモデル作成
                        // ====================================
                        DialogModel model = CreateDialogModelForSetRespotory(data["trigger_id"]);

                        // =============================
                        // ダイアログAPI実行
                        // =============================
                        HttpResponseMessage response = await slackApi.ExecutePostApiAsJson(model, "https://slack.com/api/dialog.open", data["team_id"]);
                        break;
                    }
                // ===========================
                // 登録リポジトリ照会
                // ===========================
                case "get":
                    {
                        await GetRepository(data["channel_id"], data["response_url"], data["team_id"], async (string repository) =>
                        {
                            PostMessageModel model = new PostMessageModel()
                            {
                                Channel = data["channel_id"],
                                Text = "登録リポジトリのURLを照会します" + Environment.NewLine + "https://github.com/" + repository,
                                Response_type = "ephemeral"
                            };

                            HttpResponseMessage response = await slackApi.ExecutePostApiAsJson(model, data["response_url"], data["team_id"]);
                        });
                        break;
                    }
                // ===========================
                // それ以外
                // ===========================
                default:
                    {
                        PostMessageModel model = new PostMessageModel()
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
