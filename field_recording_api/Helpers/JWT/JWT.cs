using field_recording_api.Models.AuthenticationModel;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace field_recording_api.Helpers.JWT
{
    public static class JWT
    {
        private static string secret = "o1dSYzPeMf35T3/V7y3ElgH0PSkoXF4II7tsJGEgGzU=-fi+Rze9h+Dr3hKnIA2wq1otRYrpZbhjdBoPpeZhcxyM=";//fieldrecording-summitcapital
        public static string Version;
        public static string BuildVersion;

        public static string GenerateToken(AuthenticationModel user)
        {
            byte[] key = Encoding.ASCII.GetBytes(JWT.secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);

            var _timeout = Iconfig.Configuration.GetSection("JwtTimeout:minutes").Value;
            int _addtimeout = 60;
            if (!string.IsNullOrEmpty(_timeout))
            {
                _addtimeout = int.Parse(_timeout);
            }

            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Sid, Guid.NewGuid().ToString()),
                    new Claim("username", user.usrname),
                    new Claim("loginid", user.loginid),
                    //new Claim("mkt", !String.IsNullOrEmpty(user.mkt) ? user.mkt : string.Empty ),
                    new Claim(ClaimTypes.Version, Version )
                }),
                Expires = DateTime.UtcNow.AddMinutes(_addtimeout),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha384Signature)
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);

            return handler.WriteToken(token);
        }
        public static AuthenticationModel ValidateToken(HttpContext context)
        {
            if (!context.Request.Headers.Keys.Contains("Authorization"))
            {
                throw new SecurityTokenException("Need Authorization");
            }

            string token = context.Request.Headers["Authorization"];
            token = token.Replace("Bearer ", "");

            //string token = "B46B97D2-EAB3-45EE-8C48-33CFFC857EAD";


            return DecryptionToken(token);
        }

        public static AuthenticationModel DecryptionToken(string token)
        {
            AuthenticationModel res = new AuthenticationModel();

            //res.loginid = "EFD8D2BA-38C6-435B-8066-626C2CB3C9A6";
            //res.usrname = "sangchai";

            //return res;

            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if (jwtToken == null)
                {
                    throw new SecurityTokenException("Invalid Authorization");
                }

                byte[] key = Encoding.ASCII.GetBytes(JWT.secret);
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, parameters, out securityToken);
                if (principal == null)
                {
                    throw new SecurityTokenException("Invalid ClaimsPrincipal");
                }

                var exp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(principal.FindFirst("exp").Value));
                var sdsd = exp.LocalDateTime;
                if (DateTime.Now >= sdsd)
                {
                    throw new SecurityTokenException("Expire");
                }

                Claim id = principal.FindFirst(ClaimTypes.Sid);
                if (id == null)
                {
                    throw new SecurityTokenException("Invalid ClaimType");
                }

                res.usrname = jwtToken.Claims.FirstOrDefault(c => c.Type == "username")?.Value;
                res.loginid = jwtToken.Claims.FirstOrDefault(c => c.Type == "loginid")?.Value;

                //var ddd = jwtToken.Claims.Select(d => new {d.Type, d.Value }).ToList();
                //string JsonModel = JsonConvert.SerializeObject(new AuthenticationModel());
                //string pattern = @"(?=,|{|$)[^:]+";
                
                //// Create a Regex
                //Regex rg = new Regex(pattern);
                //var keysname = rg.Matches(JsonModel).Select(d => Regex.Replace(d.Value, @"{|}|""|,", ""));
                //var ccc = jwtToken.Claims.Where(a => keysname.Contains(a.Type))
                //    .Select(aa => new { aa.Type, aa.Value }).ToList();
                //IList<string> keys = res.Properties().Select(p => p.Name).ToList();
                //JsonModel
                ////var genderObj = JsonConvert.DeserializeObject<AuthenticationModel>(JsonClaim);
                //jwtToken.mp

                //var kk = jwtToken.Claims.Where(data => data.Type)
                //jwtToken.Claims.Cast<AuthenticationModel>().FirstOrDefault();

                return res;
            }
            catch (Exception e)
            {
                throw new SecurityTokenException(e.ToString());
            }
        }

        public static class Iconfig
        {
            private static IConfiguration config;
            public static IConfiguration Configuration
            {
                get
                {
                    var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json");
                    config = builder.Build();
                    return config;
                }
            }
        }
    }
}
