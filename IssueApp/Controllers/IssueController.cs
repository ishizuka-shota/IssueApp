using IssueApp.GitHub;
using IssueApp.Models.Json;
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
                // ダイアログAPI実行
                // =============================
                HttpResponseMessage response = await slackApi.ExecutePostApiAsJson(model, "https://slack.com/api/dialog.open", data["team_id"]);
            });

           
        }
        #endregion

    }
}
