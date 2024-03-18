using System;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;



namespace ApiEsca
{
    public static class JwtManager
    {
        //cadena utilizada para generar token.
        public const string Secret = "856FECBA3B06519C8DDDBC80BB080553"; // your symetric


        /// <summary>
        /// metodo para generar token
        /// </summary>
        /// <param name="username">nombre de usuario</param>
        /// <param name="userid">usuario ID</param>
        /// <param name="expireMinutes">tiempo de vida del token</param>
        /// <returns>token</returns>
        public static string GenerateToken(string username, string userid, int expireHoras)
        {
            var symmetricKey = Convert.FromBase64String(Secret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, username),
                            new Claim(ClaimTypes.Sid, userid)
                        }),

                //Expires = now.AddMinutes(expireMinutes),
                Expires = now.AddHours(expireHoras),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            return token;
        }

        /// <summary>
        /// Metodo para obtener los claims
        /// </summary>
        /// <param name="token">token</param>
        /// <returns>retorna objeto claims</returns>
        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return null;

                var symmetricKey = Convert.FromBase64String(Secret);

                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.FromSeconds(1),
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
                };

                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

                return principal;
            }

            catch (Exception)
            {
                return null;
            }
        }
    }
}