using IssueApp.AzureTableStorage;
using IssueApp.Models.Entity;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace IssueApp.Controllers
{
    public class SlackController : ApiController
    {
        #region 【変数】Entity操作用変数(ChannelId)
        /// <summary>
        /// Entity操作用変数(CHannelId)
        /// </summary>
        private static EntityOperation<ChannelIdEntity> entityOperation_ChannelId = new EntityOperation<ChannelIdEntity>();
        #endregion


        // GET api/<controller>
        [HttpGet]
        [Route("api/slack/oauth")]
        public async Task<HttpResponseMessage> SlackOauth(string code, string state)
        {
            HttpClient client = new HttpClient();

            var query = HttpUtility.ParseQueryString(string.Empty);
            query["client_id"] = ConfigurationManager.AppSettings["slackapp_client_id"];
            query["client_secret"] = ConfigurationManager.AppSettings["slackapp_client_secret"];
            query["code"] = code;

            UriBuilder builder = new UriBuilder(new Uri("https://slack.com/api/oauth.access"))
            {
                Query = query.ToString()
            };

            HttpResponseMessage response = await client.GetAsync(builder.Uri.ToString());

            string content = await response.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(content);

            // リストから指定したラベルを基にテンプレートエンティティを作成
            //ChannelIdEntity updateEntity = entityOperation_ChannelId.RetrieveEntity(GitHubDialog.channelId, "repository", "repository");

            //updateEntity.BotToken = data["bot"]["bot_access_token"];

            // Entityで上書きする
            //ChannelIdEntity entity = entityOperation_ChannelId.UpdateEntityResult(updateEntity, string.Empty).Result as ChannelIdEntity;

            return response;

        }
    }
}
