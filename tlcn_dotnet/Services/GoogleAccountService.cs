using AutoMapper;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto.AccountDto;
using tlcn_dotnet.Dto.GoogleAccountDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IServices;
using tlcn_dotnet.ServicesImpl;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace tlcn_dotnet.Services
{
    public class GoogleAccountService: IGoogleAccountService
    {
        private readonly MyDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public GoogleAccountService(MyDbContext dbContext, IConfiguration configuration, IMapper mapper)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _mapper = mapper;
        }
        public async Task<DataResponse> LoginWithGoogle(GoogleLoginRequest request)
        {
            GoogleJsonWebSignature.Payload googlePayload;
            try
            {
                googlePayload = await GoogleJsonWebSignature.ValidateAsync(request.Token);
            }
            catch (Exception e)
            {
                throw new GeneralException("TOKEN IS INVALID", ApplicationConstant.BAD_REQUEST_CODE);
            }
            GoogleAccount googleAccount = await _dbContext.GoogleAccount
                .Include(account => account.Account)
                .Where(account => account.Email == googlePayload.Email).SingleOrDefaultAsync();
            if (googleAccount == null)
            {
                googleAccount = await InitializeGoogleAccount(googlePayload);
            }
            googleAccount.Account.Email = googleAccount.Email;
            string token = CreateJwtToken(googleAccount.Account);
            return new DataResponse(new
            {
                accessToken = token,
                user = _mapper.Map<AccountResponse>(googleAccount.Account)
            });
        }

        private string CreateJwtToken(Account account)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim("userId", account.Id.ToString()),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.Role, account.Role.ToString()),
                new Claim(ClaimTypes.MobilePhone, account.Phone ?? string.Empty),
                new Claim("status", account.Status.ToString()),
                new Claim("cityId", account.CityId ?? string.Empty),
                new Claim("districtId", account.DistrictId ?? string.Empty),
                new Claim("wardId", account.WardId ?? string.Empty),
                new Claim("detailLocation", account.DetailLocation ?? string.Empty),
                new Claim("verifyToken", account.VerifyToken ?? string.Empty),
                new Claim("firstName", account.FirstName),
                new Claim("lastName", account.LastName),
                new Claim("photoUrl", account.PhotoUrl ?? String.Empty)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                    issuer: _configuration.GetSection("Jwt:Issuer").Value,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(Double.Parse(_configuration.GetSection("Jwt:Lifetime").Value)),
                    signingCredentials: cred
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<GoogleAccount> InitializeGoogleAccount(Payload payload)
        {
            Account account = new Account()
            {
                Role = Role.ROLE_USER,
                Status = UserStatus.ACTIVE,
                CityId = string.Empty,
                DistrictId = string.Empty,
                WardId = string.Empty,
                FirstName = payload.GivenName.Split(" ")[0],
                LastName = payload.FamilyName.Split(" ")[0],
                VerifyToken = Guid.NewGuid().ToString(),
            };
            var accountDb = (await _dbContext.Account.AddAsync(account)).Entity;
            GoogleAccount googleAccount = new GoogleAccount()
            {
                Account = accountDb,
                Email = payload.Email,
            };
            var googleAccountDb = (await _dbContext.GoogleAccount.AddAsync(googleAccount)).Entity;
            await _dbContext.SaveChangesAsync();
            return googleAccountDb;
        }
    }
}
