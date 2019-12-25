using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IssueApp.Controllers
{
    public class HomeController : ApiController
    {
        [HttpGet]
        [Route("home")]
        public HttpResponseMessage Get()
        {
            return new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("ok")
            };
        }
    }
}
