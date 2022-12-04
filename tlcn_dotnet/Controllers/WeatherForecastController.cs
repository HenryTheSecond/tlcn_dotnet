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
using tlcn_dotnet.Dto.ProductDto;
using tlcn_dotnet.AuthorizationAttributes;
using tlcn_dotnet.IServices;
using tlcn_dotnet.Repositories;
using tlcn_dotnet.IRepositories;

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
        private readonly IPaymentService _paymentService;
        private readonly ICartRepository _cartRepository;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IPaymentService paymentService, ICartRepository cartRepository)
        {
            _logger = logger;
            _paymentService = paymentService;
            _cartRepository = cartRepository;
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

            /*using (StreamReader r = new StreamReader("Properties\\vietnam_city_district_ward.json"))
            {
                var result = JsonSerializer.Deserialize<JsonNode>(r.ReadToEnd());
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                foreach (var node in result["data"].AsArray())
                {
                    return Ok(node["name"].ToString());
                }
                return Ok(result["data"].AsArray());

            }  */

            return Ok
                (

                );
        }

        [HttpGet("test-upload-image")]
        public IActionResult TestUploadImage()
        {
            Account account = new Account("dihg72ez8", "778719834247269", "PDLuJVbklhnMWwR9p-GPo5gX2rA");
            Cloudinary cloudinary = new Cloudinary(account);
            cloudinary.Api.Secure = true;

            /*var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(@"https://cloudinary-devs.github.io/cld-docs-assets/assets/images/cld-sample.jpg"),
                UseFilename = true,
                UniqueFilename = false,
                Overwrite = true
            };
            var uploadResult = cloudinary.Upload(uploadParams);
            return Ok(uploadResult.Url);*/

            var deletionParams = new DeletionParams("fc958c53-09ab-448b-aaeb-abd931623c76");
            var deletionResult = cloudinary.Destroy(deletionParams);
            return Ok(deletionResult);
        }

        [HttpPost]
        public IActionResult TestUnitValidator()
        {
            var product = HttpContext.Request.Form["product"][0];
            AddProductDto addProductDto = JsonConvert.DeserializeObject<AddProductDto>(product);
            this.TryValidateModel(addProductDto, "product validate");

            return Ok(ModelState["product validate"].Errors[0].ErrorMessage);
        }

        [HttpGet("testAuthorize/{strId}")]
        [CustomAuthorize]
        public IActionResult TestAuthorize(string strId)
        {
            return Ok(true);
        }

        [HttpGet("testMomo")]
        public async Task<IActionResult> TestMomo()
        { 
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("orderId", "1");
            dict.Add("amount", 150000);
            dict.Add("orderInfo", "helloworld");
            dict.Add("requestId", "1");

            var res = await _paymentService.SendPaymentRequest(dict);
            return Ok((await res.Content.ReadFromJsonAsync<Dictionary<string, object>>())["payUrl"]);
        }

        [HttpPost("testIpn")]
        public async Task<IActionResult> TestIpn([FromBody] object body)
        {
            Console.WriteLine("IPN test successful with data: " + body);
            return Ok();
        }
    }
}