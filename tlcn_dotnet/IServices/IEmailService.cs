using MimeKit;
using tlcn_dotnet.Entity;

namespace tlcn_dotnet.Services
{
    public interface IEmailService
    {
        public Task SendRegisterConfirmationToken(ConfirmToken confirmToken);
    }
}
