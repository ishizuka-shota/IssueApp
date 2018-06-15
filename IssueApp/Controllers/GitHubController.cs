using IssueApp.AzureTableStorage;
using IssueApp.Models.Entity;
using Microsoft.WindowsAzure.Storage.Table;
using Octokit;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Security;

namespace IssueApp.Controllers
{
    public class GitHubController : ApiController
    {
        #region 【変数】GitHubクライアント変数
        /// <summary>
        /// GitHubクライアント変数
        /// </summary>
        private GitHubClient githubClient = new GitHubClient(new ProductHeaderValue("IssueApp"));
        #endregion

        // GET api/<controller>
        [HttpGet]
        [Route("api/github/oauth")]
        public async Task<RedirectResult> IssueToken(string code, string state)
        {

            //tokenのリクエストを作成
            var request = new OauthTokenRequest(ConfigurationManager.AppSettings["client_id"], ConfigurationManager.AppSettings["client_secret"], code);

            //リクエストを送信
            var token = await githubClient.Oauth.CreateAccessToken(request);

            //ユーザエンティティの操作変数作成
            EntityOperation<UserEntity> entityOperation_Template = new EntityOperation<UserEntity>();

            //作成or更新を行うユーザエンティティ作成
            UserEntity entity = new UserEntity("GitHubDialog.activity.From.Id", "GitHubDialog.activity.From.Name", token.AccessToken);

            //エンティティを操作変数を用いて作成or更新
            TableResult result = entityOperation_Template.InsertOrUpdateEntityResult(entity, "user");

            #region 未使用API送信
            ////API送信用ウェブクライアント
            //using (WebClient wc = new WebClient())
            //{
            //    //必要なクエリ情報を作成し、格納
            //    NameValueCollection nvc = new NameValueCollection();
            //    nvc.Add("client_id", ConfigurationManager.AppSettings["client_id"]);
            //    nvc.Add("client_secret", ConfigurationManager.AppSettings["client_secret"]);
            //    nvc.Add("code", code);
            //    nvc.Add("state", state);
            //    wc.QueryString = nvc;

            //    //データを送信し、また受信する
            //    byte[] response =  wc.UploadValues("https://github.com/login/oauth/access_token", nvc);

            //    //文字列化した受信バイトデータをNameValueCollectionに換装
            //    nvc = HttpUtility.ParseQueryString(wc.Encoding.GetString(response));

            //    GitHubDialog.accessToken = nvc.Get("access_token");

            //    return Redirect("https://slack.com");
            //}
            #endregion

            return Redirect("https://slack.com");

        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        [Route("api/github/login")]
        public RedirectResult LoginGitHub([FromBody]string value)
        {
            var csrf = Membership.GeneratePassword(20, 0);

            var request = new OauthLoginRequest(ConfigurationManager.AppSettings["client_id"])
            {
                Scopes = { "repo", "user" },
                State = csrf
            };

            String uri = githubClient.Oauth.GetGitHubLoginUrl(request).ToString();

            return Redirect(uri);
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}
