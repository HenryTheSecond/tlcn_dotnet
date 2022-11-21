using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.IRepositories;
using tlcn_dotnet.Services;

namespace tlcn_dotnet.AuthorizationAttributes
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            HttpContext context = filterContext.HttpContext;
            var configuration = context.RequestServices.GetService<IConfiguration>();
            var accountRepository = context.RequestServices.GetService<IAccountRepository>();

            var key = configuration.GetSection("Jwt:Key").Value;

            var jwtToken = context.Request.Headers["Authorization"].ToString().Substring("bearer ".Length);

            var tokenValidator = new JwtSecurityTokenHandler();

            object verifyTokenClaim = string.Empty;
            object id = 0;
            tokenValidator.ReadJwtToken(jwtToken).Payload.TryGetValue("verifyToken", out verifyTokenClaim);
            tokenValidator.ReadJwtToken(jwtToken).Payload.TryGetValue("userId", out id);

            if (verifyTokenClaim.ToString() != accountRepository.GetVerifyTokenById(Convert.ToInt64(id)))
            {
                filterContext.Result = new UnauthorizedObjectResult(null);
            }
        }
    }
}
