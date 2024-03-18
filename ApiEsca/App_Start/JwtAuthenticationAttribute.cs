using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace ApiEsca
{
    public class JwtAuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        public string Realm { get; set; }
        public bool AllowMultiple => false;


        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var request = context.Request;
            //DateTime fecha = DateTime.Now;
            //var date1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 6, 0, 0);
            //var date2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);

            //if (fecha >= date1 && fecha <= date2)
            //{
                var authorization = request.Headers.Authorization;

                if (authorization == null || authorization.Scheme != "Bearer")
                    return;

                if (string.IsNullOrEmpty(authorization.Parameter))
                {
                    context.ErrorResult = new AuthenticationFailureResult("Missing Jwt Token", request);
                    return;
                }

                var token = authorization.Parameter;
                var principal = await AuthenticateJwtToken(token);

                if (principal == null)
                {
                    context.ErrorResult = new AuthenticationFailureResult("Invalid token", request);
                    return;
                }
                else
                    context.Principal = principal;
            //}
            //else
            //{
            //    context.ErrorResult = new AuthenticationFailureResult("Fuera del horario de session para el sistema", request);
            //    return;
            //}
        }

        private static bool ValidateToken(string token, out string[] username)
        {
            username = new string[5];

            var simplePrinciple = JwtManager.GetPrincipal(token);
            if (simplePrinciple == null)
                return false;


            var identity = simplePrinciple.Identity as ClaimsIdentity;
            if (identity == null)
                return false;


            if (!identity.IsAuthenticated)
                return false;

            var usernameClaim = identity.FindFirst(ClaimTypes.Name);
            var useridClaim = identity.FindFirst(ClaimTypes.Sid);
            username[0] = usernameClaim?.Value;
            username[1] = useridClaim?.Value;

            if (string.IsNullOrEmpty(username[0]))
                return false;

            return true;
        }

        protected Task<IPrincipal> AuthenticateJwtToken(string token)
        {
            string[] username;

            if (ValidateToken(token, out username))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username[0]),
                    new Claim(ClaimTypes.Sid, username[1])
                };

                var identity = new ClaimsIdentity(claims, "Jwt");
                IPrincipal user = new ClaimsPrincipal(identity);

                return Task.FromResult(user);
            }

            return Task.FromResult<IPrincipal>(null);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            Challenge(context);
            return Task.FromResult(0);
        }

        private void Challenge(HttpAuthenticationChallengeContext context)
        {
            string parameter = null;

            if (!string.IsNullOrEmpty(Realm))
                parameter = "realm=\"" + Realm + "\"";

            context.ChallengeWith("Bearer", parameter);
        }
    }
}