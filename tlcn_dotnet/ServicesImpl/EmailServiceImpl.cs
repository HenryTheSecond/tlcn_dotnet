using MailKit.Net.Smtp;
using MimeKit;
using tlcn_dotnet.Constant;
using tlcn_dotnet.Entity;
using tlcn_dotnet.Services;

namespace tlcn_dotnet.ServicesImpl
{
    public class EmailServiceImpl : EmailService
    {
        private readonly IConfiguration _configuration;
        public EmailServiceImpl(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendRegisterConfirmationToken(ConfirmToken confirmToken)
        {
            var emailConfiguration = _configuration.GetSection("Email");
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(emailConfiguration.GetSection("From").Value));
            email.To.Add(MailboxAddress.Parse(confirmToken.Account.Email));
            email.Subject = ApplicationConstant.CONFIRM_TOKEN_EMAIL_SUBJECT;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            { 
                Text = EmailVerificationTemplate.emailVerificationTemplate(confirmToken.Account.Email, confirmToken.Token) 
            };
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                emailConfiguration.GetSection("Smtp").Value,
                Int32.Parse(emailConfiguration.GetSection("Port").Value),
                MailKit.Security.SecureSocketOptions.StartTls
                ); //Must use ConnectAsync not Connect, otherwise take long time to response in controller
            smtp.Authenticate(emailConfiguration.GetSection("From").Value, emailConfiguration.GetSection("Password").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
        
    }
}
