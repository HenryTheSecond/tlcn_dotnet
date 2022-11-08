using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Drawing;

namespace tlcn_dotnet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IActionResult Get()
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("phanhoang2031999@gmail.com"));
            email.To.Add(MailboxAddress.Parse("congtuyen2032001@gmail.com"));
            email.Subject = "test sending email";
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = "aaaa" };
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate("phanhoang2031999@gmail.com", "vuiyohfivxeoymnx");
            smtp.Send(email);
            smtp.Disconnect(true);
            return Ok();
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            /*List<Country> list = new List<Country>();
            using (StreamReader r = new StreamReader("Properties\\country_and_city.json"))
            using (JsonTextReader reader = new JsonTextReader(r))
            {
                reader.SupportMultipleContent = true;
                var serializer = new JsonSerializer();
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.StartObject)
                    { 
                        Country c = serializer.Deserialize<Country>(reader);
                        list.Add(c);
                    }
                }
            }
            return Ok(list);*/

            using (StreamReader r = new StreamReader("Properties\\vietnam_city_district_ward.json"))
            {
                var result = JsonSerializer.Deserialize<JsonNode>(r.ReadToEnd());
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                foreach (var node in result["data"].AsArray())
                {
                    return Ok(node["name"].ToString());
                }
                return Ok(result["data"].AsArray());

            }  
        }

        [HttpGet("test-upload-image")]
        public IActionResult TestUploadImage()
        {
            Account account = new Account("dihg72ez8", "778719834247269", "PDLuJVbklhnMWwR9p-GPo5gX2rA");
            Cloudinary cloudinary = new Cloudinary(account);
            cloudinary.Api.Secure = true;

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(@"https://cloudinary-devs.github.io/cld-docs-assets/assets/images/cld-sample.jpg"),
                UseFilename = true,
                UniqueFilename = false,
                Overwrite = true
            };
            var uploadResult = cloudinary.Upload(uploadParams);
            return Ok(uploadResult.Url);
        }
    }
}