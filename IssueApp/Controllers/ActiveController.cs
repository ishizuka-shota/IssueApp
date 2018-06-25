﻿using IssueApp.GitHub;
using IssueApp.Models.Entity;
using IssueApp.Models.Json;
using IssueApp.Models.Request;
using Newtonsoft.Json;
using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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
                        var req = JsonConvert.DeserializeObject<SlackRequest.Active<IssueCreateData>>(json["payload"]);
                        await CreateIssue(req);
                        break;
                    }
                case "displayissue":
                    {
                        var req = JsonConvert.DeserializeObject<SlackRequest.Active<Issue>>(json["payload"]);
                        await DisplayIssue(req);
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
            ChannelIdEntity entity = new ChannelIdEntity(data.Channel.Id, data.Channel.Name, data.Submission.UserName + "/" + data.Submission.Repository);

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

            PostMessageModel model = new PostMessageModel()
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
        public async Task CreateIssue(SlackRequest.Active<IssueCreateData> data)
        {
            // =============================
            // Issueオブジェクト作成
            // =============================
            NewIssue newIssue = new NewIssue(data.Submission.Title)
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
            await GetRepository(data.Channel.Id, data.Response_url, data.Team.Id, async (string repository) =>
            {
                // GitHub認証エラーハンドリング
                await AuthorizationExceptionHandler(data.Channel.Id, data.Response_url, data.Team.Id, async () =>
                {
                    // =============================
                    // Issue作成
                    // =============================
                    var issue = await GitHubApi.client.Issue.Create(repository.Split('/')[0], repository.Split('/')[1], newIssue);

                    PostMessageModel model = new PostMessageModel()
                    {
                        Channel = data.Channel.Id,
                        Text = "Issueが登録されました" + Environment.NewLine + "https://github.com/" + repository + "/issues/" + issue.Number,
                        Response_type = "in_channel",
                        Unfurl_links = true
                    };

                    HttpResponseMessage response = await slackApi.ExecutePostApiAsJson(model, data.Response_url, data.Team.Id);
                });
            });
        }
        #endregion

        #region Issue閲覧
        /// <summary>
        /// Issue閲覧
        /// </summary>
        /// <returns></returns>
        public async Task DisplayIssue(SlackRequest.Active<Issue> data)
        {
            Issue issue = null;

            // =============================
            // 登録リポジトリ取得
            // =============================
            await GetRepository(data.Channel.Id, data.Response_url, data.Team.Id, async (string repository) =>
            {
                // GitHub認証エラーハンドリング
                await AuthorizationExceptionHandler(data.Channel.Id, data.Response_url, data.Team.Id, async () =>
                {
                    issue = await GitHubApi.client.Issue.Get(repository.Split('/')[0], repository.Split('/')[1], int.Parse(data.Actions.First().Selected_options.First().Value));
                });
            });

            UpdateModel model = new UpdateModel()
            {
                Channel = data.Channel.Id,
                Ts = data.Message_ts,
                Attachments = new List<PostMessageModel.Attachment>
                {
                    new PostMessageModel.Attachment()
                    {

                    }
                }
            };

        }
        #endregion
    }
}