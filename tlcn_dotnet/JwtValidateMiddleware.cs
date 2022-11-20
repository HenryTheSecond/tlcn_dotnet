using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace tlcn_dotnet
{
    public class JwtValidateMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public JwtValidateMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context, MyDbContext dbContext)
        {
            var key = _configuration.GetSection("Jwt:Key").Value;

            var jwtToken = context.Request.Headers["Authorization"].ToString().Substring("bearer ".Length);
            var validationParameter = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration.GetSection("Jwt:Issuer").Value,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };

            ISecurityTokenValidator tokenValidator = new JwtSecurityTokenHandler();
            var claim = tokenValidator.ValidateToken(jwtToken, validationParameter, out var _);
            foreach (var cl in claim.Claims)
            { 
                Console.WriteLine(cl.Value);
            }
            await _next(context);
        }
    }
}
