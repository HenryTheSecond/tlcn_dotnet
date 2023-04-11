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
using CloudinaryDotNet;
using Account = tlcn_dotnet.Entity.Account;
using CloudinaryDotNet.Actions;
using static System.Net.Mime.MediaTypeNames;
using Role = tlcn_dotnet.Constant.Role;

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
        private readonly IChangePasswordTokenRepository _changePasswordTokenRepository;
        private readonly Cloudinary _cloudinary;

        public AuthService(IConfiguration configuration, MyDbContext dbContext, IMapper mapper, 
            IConfirmTokenService confirmTokenService, IEmailService emailService, IAccountRepository accountRepository,
            IChangePasswordTokenRepository changePasswordTokenRepository)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _mapper = mapper;
            _confirmTokenService = confirmTokenService;
            _emailService = emailService;
            _accountRepository = accountRepository;
            _changePasswordTokenRepository = changePasswordTokenRepository;

            var cloudinarySection = configuration.GetSection("Cloudinary");
            string cloudName = cloudinarySection.GetSection("CloudName").Value;
            string apiKey = cloudinarySection.GetSection("ApiKey").Value;
            string apiSecret = cloudinarySection.GetSection("ApiSecret").Value;
            CloudinaryDotNet.Account cloudinaryAccount = new CloudinaryDotNet.Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(cloudinaryAccount);
            _cloudinary.Api.Secure = true;
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

        public async Task<DataResponse> Login(LoginRequest loginRequest)
        {
            Account accountDb = await _dbContext.Account
                .Where(account => account.Email == loginRequest.Email)
                .FirstOrDefaultAsync();
            if (accountDb == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, accountDb.Password))
            {
                throw new GeneralException(ApplicationConstant.EMAIL_OR_PASSWORD_INCORRECT, ApplicationConstant.FAILED_CODE);
            }
           
            if (accountDb.Status == UserStatus.INACTIVE)
            {
                throw new GeneralException(ApplicationConstant.USER_INACTIVE, ApplicationConstant.FAILED_CODE);
            }
            return new DataResponse(new
            { 
                accessToken = CreateJwtToken(accountDb),
                user = _mapper.Map<AccountResponse>(accountDb)
            });
        }
        

        public async Task<DataResponse> RegisterAccount(RegisterAccountDto registerAccountDto, Role role = Role.ROLE_USER)
        {
            Account accountDb = _dbContext.Account.FirstOrDefault(account => account.Email == registerAccountDto.Email);
            ConfirmToken confirmToken;
            if (accountDb != null)
            {
                //if employee or admin has created a account as user role before, just need to change the role

                throw new GeneralException("Account already existed", ApplicationConstant.FAILED_CODE);
            }
            registerAccountDto.Password = BCrypt.Net.BCrypt.HashPassword(registerAccountDto.Password);
            Account account = _mapper.Map<Account>(registerAccountDto);
            account.Role = role;
            account.Status = UserStatus.INACTIVE;
            account.VerifyToken = Guid.NewGuid().ToString();

            string checkLocation = await Util.CheckVietnameseAddress(account.CityId, account.DistrictId, account.WardId);
            if (checkLocation != null)
            { 
                throw new GeneralException(checkLocation, ApplicationConstant.BAD_REQUEST_CODE);
            }
            accountDb = _dbContext.Account.Add(account).Entity;
            if (role != Role.ROLE_USER)
            {
                Employee employee = new Employee()
                {
                    Account = accountDb,
                    Id = accountDb.Id,
                    Salary = ((RegisterEmployeeDto)registerAccountDto).Salary
                };
                _dbContext.Add(employee);
            }
            await _dbContext.SaveChangesAsync();

            confirmToken = await _confirmTokenService.CreateConfirmToken(accountDb);
            _emailService.SendRegisterConfirmationToken(confirmToken);



            return new DataResponse(_mapper.Map<AccountResponse>(accountDb));
        }

        public async Task<DataResponse> GetAccount(string keyword, AccountKeywordType keywordType, string role, int page, int pageSize)
        {
            var result = await _accountRepository.GetAccount(keyword, keywordType, role, page, pageSize);

            return new DataResponse(new
            { 
                accounts = _mapper.Map<IEnumerable<AccountResponse>>(result.Accounts),
                maxPage = Util.CalculateMaxPage(result.Total, pageSize),
                currentPage = page
            });
        }

        public async Task<DataResponse> ChangePassword(ChangePasswordRequest changePasswordRequest)
        {
            Account accountDb = (await _accountRepository.FindByEmail(changePasswordRequest.Email)) ?? throw new GeneralException("EMAIL NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);

            ChangePasswordToken changePasswordToken = new ChangePasswordToken()
            {
                Account = accountDb,
                Password = BCrypt.Net.BCrypt.HashPassword(changePasswordRequest.Password),
                Token = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.Now,
                ExpireAt = DateTime.Now.AddMinutes(Double.Parse(_configuration.GetSection("ChangePassword:Duration").Value)),
                ConfirmAt = null
            };
            ChangePasswordToken changePasswordTokenDb = await _changePasswordTokenRepository.Add(changePasswordToken);
            _emailService.SendChangePasswordConfirmationToken(changePasswordTokenDb);
            return new DataResponse(true);
        }

        public async Task<DataResponse> ConfirmChangePassword(string token)
        {
            ChangePasswordToken changePasswordTokenDb = (await _changePasswordTokenRepository.FindByToken(token)) 
                ?? throw new GeneralException("TOKEN NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);
            DateTime now = DateTime.Now;
            if (changePasswordTokenDb.ConfirmAt != null)
                throw new GeneralException("TOKEN CONFIRMED", ApplicationConstant.FAILED_CODE);
            if (now > changePasswordTokenDb.ExpireAt)
                throw new GeneralException(ApplicationConstant.TOKEN_EXPIRED, ApplicationConstant.FAILED_CODE);
            changePasswordTokenDb.ConfirmAt = now;
            changePasswordTokenDb.Account.Password = changePasswordTokenDb.Password;
            changePasswordTokenDb.Account.VerifyToken = Guid.NewGuid().ToString(); 
            ChangePasswordToken changePasswordTokenSaved = await _changePasswordTokenRepository.Update(changePasswordTokenDb);
            return new DataResponse(_mapper.Map<AccountResponse>(changePasswordTokenSaved.Account));
        }

        public async Task<DataResponse> GetAccountById(long id)
        {
            Account account = (await _accountRepository.GetById(id)) 
                ?? throw new GeneralException("ACCOUNT NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);
            return new DataResponse(_mapper.Map<AccountResponse>(account));
        }

        public async Task<DataResponse> GetProfile(string authorization)
        {
            long accountId = Util.ReadJwtTokenAndGetAccountId(authorization);
            Account account = await _accountRepository.GetById(accountId);
            return new DataResponse(_mapper.Map<AccountResponse>(account));
        }

        public async Task<DataResponse> UploadPhoto(string authorization, IFormFile photo)
        {
            long accountId = Util.ReadJwtTokenAndGetAccountId(authorization);

            using (var stream = new MemoryStream())
            {
                await photo.CopyToAsync(stream);
                stream.Seek(0, SeekOrigin.Begin);
                string fileName = $"user_photo_{accountId}";
                ImageUploadParams uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(fileName, stream),
                    UseFilename = true,
                    UniqueFilename = false,
                    Overwrite = true
                };
                ImageUploadResult result = await _cloudinary.UploadAsync(uploadParams);
                Account accountDb = await _accountRepository.GetById(accountId);
                accountDb.PhotoUrl = result.Url.ToString();
                await _accountRepository.Update(accountDb);
                return new DataResponse(_mapper.Map<AccountResponse>(accountDb));
            }

        }

        public async Task<DataResponse> UpdateProfile(string authorization, UpdateProfileRequest request)
        {
            long accountId = Util.ReadJwtTokenAndGetAccountId(authorization);

            var checkLocation = await Util.CheckVietnameseAddress(request.CityId, request.DistrictId, request.WardId);
            if (checkLocation != null)
                return new DataResponse
                {
                    Message = checkLocation,
                    Status = ApplicationConstant.BAD_REQUEST_CODE
                };
            Account account = await _accountRepository.GetById(accountId);
            account.Phone = request.Phone != null ? request.Phone : account.Phone;
            account.CityId = request.CityId;
            account.DistrictId = request.DistrictId;
            account.WardId = request.WardId;
            account.DetailLocation = request.DetailLocation != null ? request.DetailLocation : account.DetailLocation;
            account.FirstName = request.FirstName != null ? request.FirstName : account.FirstName;
            account.LastName = request.LastName != null ? request.LastName : account.LastName;

            Account accountDb = await _accountRepository.Update(account);
            return new DataResponse(_mapper.Map<AccountResponse>(accountDb));
        }

        public async Task<DataResponse> UpdateAccountRole(UpdateRoleRequest request)
        {
            Account account = await _accountRepository.FindByEmail(request.Email);
            if(account == null)
                throw new GeneralException("ACCOUNT NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);
            Role oldRole = account.Role;
            account.Role = request.Role;
            if (oldRole != Role.ROLE_USER && request.Role == Role.ROLE_USER)
            {
                Employee employee = _dbContext.Employee.Find(account.Id);
                if (employee != null)
                    _dbContext.Employee.Remove(employee);
            }
            else if (oldRole == Role.ROLE_USER && request.Role != Role.ROLE_USER)
            {
                Employee employee = new Employee
                {
                    Account = account,
                    Id = account.Id,
                    Salary = request.Salary
                };
                _dbContext.Employee.Add(employee);
            }
            else if (oldRole != Role.ROLE_USER && request.Role != Role.ROLE_USER)
            {
                Employee employee = _dbContext.Employee.Find(account.Id);
                if (employee != null)
                {
                    employee.Salary = request.Salary != null ? request.Salary : employee.Salary;
                    _dbContext.Update(employee);
                }
            }
            _dbContext.SaveChanges();
            return new DataResponse(_mapper.Map<AccountResponse>(account));
        }

        public async Task<DataResponse> AdminManageEmployee(AdminManageEmployeeRequest request)
        {
            var query = _dbContext.Account.AsQueryable();
            if(request.Role == null)
            {
                query = query.Where(account => account.Role == Role.ROLE_EMPLOYEE || account.Role == Role.ROLE_ADMIN);
            }
            else
            {
                query = query.Where(account => account.Role == request.Role);
            }
            if(request.SearchBy != null)
            {
                request.Keyword = request.Keyword == null ? string.Empty : request.Keyword;
                switch(request.SearchBy.Value)
                {
                    case ManageEmployeeSearchBy.EMAIL:
                        query = query.Where(account => account.Email.Contains(request.Keyword));
                        break;
                    case ManageEmployeeSearchBy.PHONE:
                        query = query.Where(account => account.Phone.Contains(request.Keyword));
                        break;
                    case ManageEmployeeSearchBy.NAME:
                        query = query.Where(account => (account.LastName + " " + account.FirstName).Contains(request.Keyword));
                        break;
                }
            }
            int count = await query.CountAsync();
            var result = await query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
            return new DataResponse(new
            {
                Accounts = _mapper.Map<List<AccountResponse>>(result),
                Total = count,
                MaxPage = Util.CalculateMaxPage(count, request.PageSize),
                CurrentPage = request.Page
            });
        }
    }
}
