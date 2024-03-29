﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NLog;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Text;
using tlcn_dotnet.Constant;
using tlcn_dotnet.Controllers;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;
using tlcn_dotnet.Repositories;
using tlcn_dotnet.Utils;

namespace tlcn_dotnet
{
    public static class ServiceExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, Logger? logger)
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
                    else if (errorType == typeof(SqlException))
                    {
                        SqlException sqlException = (SqlException)contextFeature.Error;
                        dataResponse = ExceptionUtil.SqlExceptionHandle(sqlException);
                    }
                    else
                    {
                        dataResponse.Status = Constant.ApplicationConstant.FAILED_CODE;
                        dataResponse.Message = Constant.ApplicationConstant.FAILED;
                        logger.Error(contextFeature.Error.StackTrace);
                    }

                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                    {
                        message = dataResponse.Message,
                        data = dataResponse.Data,
                        status = dataResponse.Status,
                        detailMessage = dataResponse.DetailMessage
                    }));
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
