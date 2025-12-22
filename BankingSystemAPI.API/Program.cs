using System.Security.Claims;
using System.Text;
using BankingSystemAPI.API.Middleware;
using BankingSystemAPI.Core.Enums;
using BankingSystemAPI.Core.Interfaces;
using BankingSystemAPI.Core.Interfaces.Application;
using BankingSystemAPI.Core.Interfaces.Infrastructure;
using BankingSystemAPI.Core.Interfaces.Persistence;
using BankingSystemAPI.Core.settings;
using BankingSystemAPI.Data;
using BankingSystemAPI.Data.Context;
using BankingSystemAPI.Data.Repositories;
using BankingSystemAPI.Services.Helpers;
using BankingSystemAPI.Services.Services;
using BankingSystemAPI.Services.Services.Infrastructure.CurrencyExchange;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
    

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "BankingSystemAPI", Version = "v1" });

    // JWT Bearer authentication
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Enter your JWT token here. Example: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddSingleton<JwtTokenGenerator>();

//custom Authorization
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("VerifiedCustomerOnly", policy =>
    {
        policy.RequireAuthenticatedUser();

        policy.RequireAssertion(context =>
        {
            var verificationStatus =
                context.User.FindFirst("verificationStatus")?.Value;

            var role =
                context.User.FindFirst(ClaimTypes.Role)?.Value;

            var isActive =
                context.User.FindFirst("isActive")?.Value;
            
            return
                role == "Customer" &&
                verificationStatus == nameof(CustomerVerificationStatus.Verified) &&
                isActive == "true";
        });
    });

//app_settings.json
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<RefreshTokenSetting>(
    builder.Configuration.GetSection("RefreshTokenSettings")
);
//Register DbContext
builder.Services.AddDbContext<BankingSystemDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

// Register services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IEmailVerificationTokenService, EmailVerificationTokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IAccountService, AccountService>();

//Infrastructure
builder.Services.AddHttpClient<ICurrencyExchangeService, CurrencyExchangeService>();

builder.Services.AddHttpContextAccessor();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "BankingSystemAPI v1");
        });
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseMiddleware<TokenVersionMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();