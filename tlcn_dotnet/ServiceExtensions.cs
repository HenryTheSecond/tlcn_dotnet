using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text;
using tlcn_dotnet.CustomException;

namespace tlcn_dotnet
{
    public static class ServiceExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(error =>
            {
                error.Run(async context =>
                {
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    DataResponse dataResponse = new DataResponse();
                    Type errorType = contextFeature.Error.GetType();
                    if (errorType == typeof(GeneralException))
                    {
                        GeneralException generalException = (GeneralException)contextFeature.Error;
                        dataResponse.Message = generalException.Message;
                        dataResponse.Status = generalException.Status == null ? dataResponse.Status : generalException.Status.Value;
                        dataResponse.DetailMessage = generalException.DetailMessage == null ? dataResponse.DetailMessage : generalException.DetailMessage;
                        dataResponse.Data = generalException.Data == null ? dataResponse.Data : generalException.Data;
                    }
                    else
                    {
                        dataResponse.Status = Constant.ApplicationConstant.FAILED_CODE;
                        dataResponse.Message = Constant.ApplicationConstant.FAILED;
                    }

                    await context.Response.WriteAsync(JsonConvert.SerializeObject(dataResponse));
                });
            });
        }

        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            var key = configuration.GetSection("Jwt:Key").Value;

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration.GetSection("Jwt:Issuer").Value,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };
            });
        }
    }
}
