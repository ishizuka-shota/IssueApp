using IssueApp.GitHub;
using IssueApp.Models.Json;
using Octokit;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using static IssueApp.Common.CommonUtility;
using static IssueApp.Common.ErrorHandler;
using static IssueApp.Common.RepositoryOperation;
using static IssueApp.Models.ModelOperation;

namespace IssueApp.Controllers
{
    public class IssueController : ApiController
    {
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
            NameValueCollection data = await GetBody(request);

            // =============================
            // GitHubアクセストークンの設定
            // =============================
            GitHubApi.SetCredential(data["user_id"]);

            // 引数の値
            string method = data["text"];

            // ===========================
            // 引数の値でダイアログ表示を分ける
            // ===========================
            switch (method)
            {
                // ===========================
                // Issue閲覧ダイアログ
                // ===========================
                case "display":
                    {
                        await DisplayIssue(data);
                        break;
                    }
                // ===========================
                // Issue作成ダイアログ
                // ===========================
                case "create":
                    {
                        await CreateIssue(data);
                        break;
                    }
            }      
        }
        #endregion

        #region Issue閲覧メッセージ表示
        /// <summary>
        /// Issue閲覧メッセージ表示
        /// </summary>
        /// <returns></returns>
        public async Task DisplayIssue(NameValueCollection data)
        {
            List<Issue> issueList = null;

            // =============================
            // 登録リポジトリ照会
            // =============================
            await GetRepository(data["channel_id"], data["response_url"], data["team_id"], async (string repository) =>
            {
                // GitHub認証エラーハンドリング
                await AuthorizationExceptionHandler(data["channel_id"], data["response_url"], data["team_id"], async () =>
                {
                    var issueListProtoType = await GitHubApi.client.Issue.GetAllForRepository(repository.Split('/')[0], repository.Split('/')[1]);

                    issueList = issueListProtoType.ToList();
                });
            });

            // ===================================
            // Issue閲覧用メッセージ表示モデル作成
            // ===================================
            PostMessageModel model = CreatePostMessageModelForDisplayIssue(data["channel_id"], issueList);

            // =============================
            // API実行
            // =============================
            HttpResponseMessage response = await slackApi.ExecutePostApiAsJson(model, data["response_url"], data["team_id"]);

        }
        #endregion

        #region Issue作成ダイアログ表示
        /// <summary>
        /// Issue作成ダイアログ表示
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task CreateIssue(NameValueCollection data)
        {
            // =============================
            // 登録リポジトリ照会
            // =============================
            await GetRepository(data["channel_id"], data["response_url"], data["team_id"], async (string repository) =>
            {
                List<string> labelNameList = null;

                // GitHub認証エラーハンドリング
                await AuthorizationExceptionHandler(data["channel_id"], data["response_url"], data["team_id"], async () =>
                {
                    // クライアントを用いてリポジトリ名からIssueのラベルを取得
                    var labelList = await GitHubApi.client.Issue.Labels.GetAllForRepository(repository.Split('/')[0], repository.Split('/')[1]);

                    // ラベル変数リストを文字列リストに変換
                    labelNameList = labelList.ToList().ConvertAll(x => x.Name);
                });

                // ===============================
                // Issue作成用ダイアログモデル作成
                // ===============================
                DialogModel model = CreateDialogModelForCreateIssue(data["trigger_id"], labelNameList);

                // =============================
                // API実行
                // =============================
                HttpResponseMessage response = await slackApi.ExecutePostApiAsJson(model, "https://slack.com/api/dialog.open", data["team_id"]);
            });
        }
        #endregion

    }
}
