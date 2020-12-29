using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApp1.Controllers
{
    public class CustomIdentity : IIdentity
    {
        private string _name;
        private string _authenticationType;
        private bool _isAuthenticated;

        public CustomIdentity(string name, string authenticationType, bool isAuthenticated)
        {
            _name = name;
            _authenticationType = authenticationType;
            _isAuthenticated = isAuthenticated;
        }

        public string Name { get { return _name; } }
        public string AuthenticationType { get { return _authenticationType; } }
        public bool IsAuthenticated { get { return _isAuthenticated; } }
    }

    public class UploadController : ApiController
    {

        

        public ClaimsPrincipal ClaimsPrincipal
        {
            get
            {
                var t = Thread.CurrentPrincipal as ClaimsPrincipal;
                return t;
            }
            set
            {
                Thread.CurrentPrincipal = value;
            }
        }

        [HttpPost]
        [Route("api/upload")]
        public async Task<HttpResponseMessage> PostFileUpload()
        {
            // perform simple apikey authentication
            ApiKeyAuthenticate(Request);

            if (!Request.Content.IsMimeMultipartContent())
                return Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, "Media type not supported");

            // we will capture the Current Principal before executing ReadAsMultipartAsync()
            var currentPrincipalBefore = Thread.CurrentPrincipal as ClaimsPrincipal;

            // BUG 1217194 - Executing this step when uploading a large file
            //               causes the Thread.CurrentPrincipal to be erased
            //               and the current claims in that identity are lost
            var streamProvider = await Request.Content.ReadAsMultipartAsync();

            // we will capture the Current Principal after executing ReadAsMultipartAsync()
            var currentPrincipalAfter = Thread.CurrentPrincipal as ClaimsPrincipal;

            // if the Current Principal has been altered where the claims it contained are removed
            if (currentPrincipalBefore != null
                && currentPrincipalBefore.Claims.Count() > 0
                && currentPrincipalAfter != null
                && currentPrincipalAfter.Claims.Count() == 0)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred with the CurrentPrincipal");
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private void ApiKeyAuthenticate(HttpRequestMessage request)
        {
            if (request.Headers.Contains("ApiKey"))
            {
                var apiKey = request.Headers.FirstOrDefault(x => x.Key == "ApiKey").Value.FirstOrDefault();
                if (apiKey != null)
                {
                    if (apiKey == "1234")
                    {
                        Thread.CurrentPrincipal = CreateThreadIdentity();
                    }
                }
            }
        }

        private ClaimsPrincipal CreateThreadIdentity()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(ClaimTypes.NameIdentifier, "SomeValueHere"),
                                        new Claim(ClaimTypes.Name, "gunnar@somecompany.com")
                                        // other required and custom claims
                                   }, "TestAuthentication"));

            return user;
        }

    }
}