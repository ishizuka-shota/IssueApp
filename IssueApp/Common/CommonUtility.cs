using IssueApp.AzureTableStorage;
using IssueApp.Models.Entity;
using IssueApp.Slack;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace IssueApp.Common
{
    /// <summary>
    /// 共通操作クラス
    /// </summary>
    public static class CommonUtility
    {
        #region 【変数】SlackApi実行用変数
        /// <summary>
        /// SlackApi実行用変数
        /// </summary>
        public static SlackApi slackApi = new SlackApi();
        #endregion

        #region 【変数】Entity操作用変数(ChannelId)
        /// <summary>
        /// Entity操作用変数(ChannelId)
        /// </summary>
        public static EntityOperation<ChannelIdEntity> entityOperation_ChannelId = new EntityOperation<ChannelIdEntity>();
        #endregion

        #region 【変数】Entity操作用変数(TeamId)
        /// <summary>
        /// Entity操作用変数(TeamId)
        /// </summary>
        public static EntityOperation<TeamIdEntity> entityOperation_TeamId = new EntityOperation<TeamIdEntity>();
        #endregion


        #region Body値を取得
        /// <summary>
        /// Body値を取得
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<NameValueCollection> GetBody(HttpRequestMessage request)
        {
            string content = await request.Content.ReadAsStringAsync();
            return HttpUtility.ParseQueryString(content);
        }
        #endregion
    }
}