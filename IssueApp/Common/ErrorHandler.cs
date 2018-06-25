﻿using IssueApp.Models.Json;
using IssueApp.Slack;
using Octokit;
using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;

namespace IssueApp.Common
{
    /// <summary>
    /// エラーハンドリング用クラス
    /// </summary>
    public static class ErrorHandler
    {
        #region 【変数】SlackApi実行用変数
        /// <summary>
        /// SlackApi実行用変数
        /// </summary>
        private static SlackApi slackApi = new SlackApi();
        #endregion

        #region 【デリゲート】処理
        /// <summary>
        /// 処理
        /// </summary>
        /// <returns></returns>
        public delegate Task Canceler();
        #endregion

        #region エラーハンドラー
        /// <summary>
        /// エラーハンドラー
        /// </summary>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public static async Task Handler(NameValueCollection data, Canceler cancel)
        {
            try
            {
                //なにかしらの処理
                await cancel();
            }
            //操作キャンセル時の処理
            catch (OperationCanceledException)
            {
              
            }
            //ユーザ情報不正時の処理
            catch (AuthorizationException)
            {
                PostMessageModel model = new PostMessageModel()
                {
                    Channel = data["channel_id"],
                    Text = "ユーザ情報が不正、もしくは存在しません。",
                    Response_type = "ephemeral"
                };

                HttpResponseMessage response = await slackApi.ExecutePostApiAsJson(model, data["response_url"], data["team_id"]);
            }
            //それ以外の例外処理
            catch (Exception)
            {
                PostMessageModel model = new PostMessageModel()
                {
                    Channel = data["channel_id"],
                    Text = "予期せぬエラーが発生しました。",
                    Response_type = "ephemeral"
                };

                HttpResponseMessage response = await slackApi.ExecutePostApiAsJson(model, data["response_url"], data["team_id"]);
            }
        }
        #endregion

        #region GitHubユーザ認証エラーハンドリング
        /// <summary>
        /// GitHubユーザ認証エラーハンドリング
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public static async Task AuthorizationExceptionHandler(string channelId, string response_url, string teamId, Func<Task> func)
        {
            try
            {
                await func();
            }
            catch (AuthorizationException)
            {
                PostMessageModel model = new PostMessageModel()
                {
                    Channel = channelId,
                    Text = "ユーザ情報が不正、もしくは存在しません。",
                    Response_type = "ephemeral"
                };

                HttpResponseMessage response = await slackApi.ExecutePostApiAsJson(model, response_url, teamId);
            }
        }
        #endregion
    }
}