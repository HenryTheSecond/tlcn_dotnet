using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace tlcn_dotnet
{
    public class SecurityTokenValidator : ISecurityTokenValidator
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SecurityTokenValidator(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public bool CanValidateToken => throw new NotImplementedException();

        public int MaximumTokenSizeInBytes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool CanReadToken(string securityToken)
        {
            return true;
        }

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {

            /*var httpContext = _httpContextAccessor.HttpContext;
            StringValues token = new StringValues();
            httpContext.Response.Headers.TryGetValue("Authorization", out token);
            Console.WriteLine(token);*/
            Console.WriteLine(_httpContextAccessor == null);
            validatedToken = null;
            return null;
        }
    }
}
