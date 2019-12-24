using IssueApp.GitHub;
using IssueApp.Models.Json;
using System.Collections.Generic;
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
            var data = await GetBody(request);

            // 引数の値
            var method = data["text"];


            // ===========================
            // 引数の値で処理を分ける
            // ===========================
            switch (method)
            {
                // ===========================
                // issue登録
                // ===========================
                case "set":
                {
                    // =============================
                    // GitHubアクセストークンの設定
                    // =============================
                    GitHubApi.SetCredential(data["user_id"]);

                    // =============================
                    // 登録リポジトリ照会
                    // =============================

                    await GetRepository(data["channel_id"], data["response_url"], data["team_id"], async repository =>
                    {
                        List<string> labelNameList = null;

                        // GitHub認証エラーハンドリング
                        await AuthorizationExceptionHandler(data["channel_id"], data["response_url"], data["team_id"],
                            async () =>
                            {
                                // クライアントを用いてリポジトリ名からIssueのラベルを取得
                                var labelList =
                                    await GitHubApi.Client.Issue.Labels.GetAllForRepository(repository.Split('/')[0],
                                        repository.Split('/')[1]);

                                // ラベル変数リストを文字列リストに変換
                                labelNameList = labelList.ToList().ConvertAll(x => x.Name);
                            });

                        // ===============================
                        // Issue作成用ダイアログモデル作成
                        // ===============================
                        DialogModel model = CreateDialogModelForCreateIssue(data["trigger_id"], labelNameList);

                        // =============================
                        // ダイアログAPI実行
                        // =============================
                        await SlackApi.ExecutePostApiAsJson(model, "https://slack.com/api/dialog.open",
                            data["team_id"]);
                    });
                    break;
                }
                // ===========================
                // それ以外
                // ===========================
                default:
                {
                    var model = new PostMessageModel()
                    {
                        Channel = data["channel_id"],
                        Text = "引数を入力してください",
                        Response_type = "ephemeral"
                    };

                    await SlackApi.ExecutePostApiAsJson(model, data["response_url"], data["team_id"]);

                    break;
                }

            }

            #endregion
        }
    }
}
