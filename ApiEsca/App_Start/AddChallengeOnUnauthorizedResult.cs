using Newtonsoft.Json;
//using Newtonsoft.Json;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace ApiEsca
{
    public class AddChallengeOnUnauthorizedResult : IHttpActionResult
    {
        public AuthenticationHeaderValue Challenge { get; }
        public IHttpActionResult InnerResult { get; }


        public AddChallengeOnUnauthorizedResult(
            AuthenticationHeaderValue challenge, 
            IHttpActionResult innerResult
            )
        {
            Challenge = challenge;
            InnerResult = innerResult;
        }



        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await InnerResult.ExecuteAsync(cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {

                // Only add one challenge per authentication scheme.
                if (response.Headers.WwwAuthenticate.All(h => h.Scheme != Challenge.Scheme))
                {
                    response.Headers.WwwAuthenticate.Add(Challenge);
                }

                if (response.Content == null)
                {
                    var objeto = new
                    {
                        error = 401,
                        menssaje = "error bad token or time expired."
                    };
                    var newResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    var jss = JsonConvert.SerializeObject(objeto);
                    newResponse.Content = new StringContent(jss, Encoding.UTF8, "application/json");
                    return newResponse;
                }
            }

            return response;
        }
    }
}