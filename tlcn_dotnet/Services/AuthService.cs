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
using tlcn_dotnet.IRepositories;

namespace tlcn_dotnet.ServicesImpl
{
    public class AuthService: IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly MyDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IConfirmTokenService _confirmTokenService;
        private readonly IEmailService _emailService;
        private readonly IAccountRepository _accountRepository;

        public AuthService(IConfiguration configuration, MyDbContext dbContext, IMapper mapper, 
            IConfirmTokenService confirmTokenService, IEmailService emailService, IAccountRepository accountRepository)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _mapper = mapper;
            _confirmTokenService = confirmTokenService;
            _emailService = emailService;
            _accountRepository = accountRepository;
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

        private string CreateJwtToken(Account account)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.Role, account.Role.ToString()),
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

        public async Task<DataResponse> Login(LoginRequest loginRequest)
        {
            Account accountDb = await _dbContext.Account.Where(account => account.Email == loginRequest.Email).FirstOrDefaultAsync();
            if (accountDb == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, accountDb.Password))
            {
                throw new GeneralException(ApplicationConstant.EMAIL_OR_PASSWORD_INCORRECT, ApplicationConstant.FAILED_CODE);
            }
           
            if (accountDb.Status == UserStatus.INACTIVE)
            {
                throw new GeneralException(ApplicationConstant.USER_INACTIVE, ApplicationConstant.FAILED_CODE);
            }
            return new DataResponse(CreateJwtToken(accountDb));
        }
        

        public async Task<DataResponse> RegisterAccount(RegisterAccountDto registerAccountDto, Role role = Role.ROLE_USER)
        {
            Account accountDb = _dbContext.Account.FirstOrDefault(account => account.Email == registerAccountDto.Email);
            ConfirmToken confirmToken;
            if (accountDb != null)
            {
                //if employee or admin has created a account as user role before, just need to change the role
                if (role == Role.ROLE_EMPLOYEE || role == Role.ROLE_ADMIN)
                {
                    accountDb.Role = role;
                    if (accountDb.Status == UserStatus.INACTIVE)
                    {
                        confirmToken = await _confirmTokenService.CreateConfirmToken(accountDb);
                        _emailService.SendRegisterConfirmationToken(confirmToken);
                    }
                    _dbContext.SaveChanges();
                    return new DataResponse(_mapper.Map<AccountResponse>(accountDb));
                }

                throw new GeneralException("Account already existed", ApplicationConstant.FAILED_CODE);
            }
            registerAccountDto.Password = BCrypt.Net.BCrypt.HashPassword(registerAccountDto.Password);
            Account account = _mapper.Map<Account>(registerAccountDto);
            account.Role = role;
            account.Status = UserStatus.INACTIVE;

            string checkLocation = await Util.CheckVietnameseAddress(account.CityId, account.DistrictId, account.WardId);
            if (checkLocation != null)
            { 
                throw new GeneralException(checkLocation, ApplicationConstant.BAD_REQUEST_CODE);
            }
            accountDb = _dbContext.Account.Add(account).Entity;
            await _dbContext.SaveChangesAsync();

            confirmToken = await _confirmTokenService.CreateConfirmToken(accountDb);
            _emailService.SendRegisterConfirmationToken(confirmToken);

            return new DataResponse(_mapper.Map<AccountResponse>(accountDb));
        }

        public async Task<DataResponse> FilterAccount(string keyword, AccountKeywordType keywordType, string role, int page)
        {
            var result = await _accountRepository.FilterAccount(keyword, keywordType, role, page);

            return new DataResponse(new
            { 
                accounts = _mapper.Map<IEnumerable<AccountResponse>>(result.Accounts),
                maxPage = Util.CalculateMaxPage(result.Total, 2),
                currentPage = page
            });
        }
    }
}
