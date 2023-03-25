using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using Swashbuckle.AspNetCore.Filters;
using System.Configuration;
using System.Text;
using tlcn_dotnet;
using tlcn_dotnet.DatabaseContext;
using tlcn_dotnet.IRepositories;
using tlcn_dotnet.IServices;
using tlcn_dotnet.Repositories;
using tlcn_dotnet.RepositoriesImpl;
using tlcn_dotnet.Services;
using tlcn_dotnet.ServicesImpl;



var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();
    builder.Services.AddCors(p => p.AddPolicy("FrontEnd", build =>
    {
        build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
    }));

    builder.Services.AddDbContext<MyDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    builder.Services.AddSingleton<DapperContext>();


    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
        });
        options.OperationFilter<SecurityRequirementsOperationFilter>();
    });

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddAutoMapper(typeof(Program).Assembly);
    builder.Services.AddSingleton<HttpClient>();

    //Add services
    builder.Services.AddScoped<ILocationService, LocationService>(); //Should AddSingleton instead maybe
    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IConfirmTokenService, ConfirmTokenService>();
    builder.Services.AddScoped<IEmailService, EmailService>();
    builder.Services.AddScoped<ISupplierService, SupplierService>();
    builder.Services.AddScoped<IProductService, ProductService>();
    builder.Services.AddScoped<IProductImageService, ProductImageService>();
    builder.Services.AddScoped<IInventoryService, InventoryService>();
    builder.Services.AddScoped<ICartDetailService, CartDetailService>();
    builder.Services.AddScoped<IBillService, BillService>();
    builder.Services.AddScoped<ICartService, CartService>();
    builder.Services.AddScoped<IPaymentService, MomoPaymentService>();
    builder.Services.AddScoped<IReviewService, ReviewService>();
    builder.Services.AddScoped<IDeliveryService, GhnDeliveryService>();
    builder.Services.AddScoped<IStatisticsService, StatisticsService>();

    //Add repositories
    builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
    builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
    builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
    builder.Services.AddScoped<IAccountRepository, AccountRepository>();
    builder.Services.AddScoped<IChangePasswordTokenRepository, ChangePasswordTokenRepository>();
    builder.Services.AddScoped<ICartDetailRepository, CartDetailRepository>();
    builder.Services.AddScoped<ICartRepository, CartRepository>();
    builder.Services.AddScoped<IBillRepository, BillRepository>();
    builder.Services.AddScoped<IBillDetailRepository, BillDetailRepository>();
    builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
    builder.Services.AddScoped<IStatisticsRepository, StatisticsRepository>();

    /*builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options => {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt:Key").Value)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });*/
    builder.Services.ConfigureJWT(builder.Configuration); //Jwt configure

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.ConfigureExceptionHandler(logger); //Exception Handler

    app.UseHttpsRedirection();
    app.UseCors("FrontEnd");

    app.UseAuthentication();
    app.UseAuthorization();
    //app.UseMiddleware<JwtValidateMiddleware>();

    app.MapControllers();

    app.Run();
}
catch (Exception e)
{
    logger.Error(e);
    throw e;
}
finally
{
    NLog.LogManager.Shutdown();
}
