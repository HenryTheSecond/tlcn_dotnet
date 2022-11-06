using AutoMapper;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Nodes;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto.AccountDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.Services;
using tlcn_dotnet.Utils;
using Microsoft.EntityFrameworkCore;

namespace tlcn_dotnet.ServicesImpl
{
    public class AuthServiceImpl: AuthService
    {
        private readonly IConfiguration _configuration;
        private readonly MyDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ConfirmTokenService _confirmTokenService;
        private readonly EmailService _emailService;

        public AuthServiceImpl(IConfiguration configuration, MyDbContext dbContext, IMapper mapper, 
            ConfirmTokenService confirmTokenService, EmailService emailService)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _mapper = mapper;
            _confirmTokenService = confirmTokenService;
            _emailService = emailService;
        }

        public async Task<DataResponse> ConfirmAccount(string token)
        {
            ConfirmToken confirmToken = _dbContext.ConfirmToken.Include(confirmToken => confirmToken.Account).FirstOrDefault(confirmToken => confirmToken.Token == token);
            if (confirmToken == null)
                throw new GeneralException(ApplicationConstant.TOKEN_NOT_FOUND, ApplicationConstant.NOT_FOUND_CODE);
            if(confirmToken.ConfirmAt != null)
                throw new GeneralException(ApplicationConstant.EMAIL_HAS_BEEN_CONFIRMED, ApplicationConstant.FAILED_CODE);
            DateTime now = DateTime.Now;
            if(now > confirmToken.ExpireAt)
                throw new GeneralException(ApplicationConstant.TOKEN_EXPIRED, ApplicationConstant.FAILED_CODE);
            confirmToken.ConfirmAt = now;
            confirmToken.Account.Status = UserStatus.ACTIVE;
            await _dbContext.SaveChangesAsync();
            return new DataResponse(_mapper.Map<AccountResponse>(confirmToken.Account));
        }

        public async Task<DataResponse> CreateToken()
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "congtuyen2032001@gmail.com"),
                new Claim(ClaimTypes.Role, Role.ROLE_ADMIN.ToString()),
                new Claim(ClaimTypes.Role, Role.ROLE_EMPLOYEE.ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                    issuer: _configuration.GetSection("Jwt:Issuer").Value,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(Double.Parse(_configuration.GetSection("Jwt:Lifetime").Value)),
                    signingCredentials: cred
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return new DataResponse(jwt);
        }

        public async Task<DataResponse> RegisterUser(RegisterAccountDto registerAccountDto)
        {
            if (_dbContext.Account.FirstOrDefault(account => account.Email == registerAccountDto.Email) != null)
            {
                throw new GeneralException("Account already existed", ApplicationConstant.FAILED_CODE);
            }
            registerAccountDto.Password = BCrypt.Net.BCrypt.HashPassword(registerAccountDto.Password);
            Account account = _mapper.Map<Account>(registerAccountDto);
            account.Role = Role.ROLE_USER;
            account.Status = UserStatus.INACTIVE;

            string checkLocation = await Util.CheckVietnameseAddress(account.CityId, account.DistrictId, account.WardId);
            if (checkLocation != null)
            { 
                throw new GeneralException(checkLocation, ApplicationConstant.BAD_REQUEST_CODE);
            }
            Account accountDb = _dbContext.Account.Add(account).Entity;
            await _dbContext.SaveChangesAsync();
            ConfirmToken confirmToken = await _confirmTokenService.CreateConfirmToken(accountDb);

            var watch = System.Diagnostics.Stopwatch.StartNew();
            _emailService.SendRegisterConfirmationToken(confirmToken);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine(elapsedMs);

            return new DataResponse(_mapper.Map<AccountResponse>(accountDb));
        }

    }
}
