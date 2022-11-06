using tlcn_dotnet.Entity;
using tlcn_dotnet.Services;

namespace tlcn_dotnet.ServicesImpl
{
    public class ConfirmTokenServiceImpl : ConfirmTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly MyDbContext _dbContext;

        public ConfirmTokenServiceImpl(IConfiguration configuration, MyDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }
        public async Task<ConfirmToken> CreateConfirmToken(Account account)
        {
            string token = Guid.NewGuid().ToString();
            DateTime createAt = DateTime.Now;
            DateTime expireAt = DateTime.Now.AddMinutes(Double.Parse(_configuration.GetSection("ConfirmToken:Duration").Value));
            ConfirmToken confirmToken = new ConfirmToken() 
            {
                Token = token,
                CreateAt = createAt,
                ExpireAt = expireAt,
                Account = account
            };
            ConfirmToken confirmTokenDb = (await _dbContext.ConfirmToken.AddAsync(confirmToken)).Entity;
            await _dbContext.SaveChangesAsync();
            return confirmTokenDb;
        }
    }
}
