using System.Text;
using BankingSystemAPI.Core.Interfaces.Repositories;
using BankingSystemAPI.Core.Interfaces.Services;
using BankingSystemAPI.Core.settings;
using BankingSystemAPI.Data.Context;
using BankingSystemAPI.Data.Repositories;
using BankingSystemAPI.Services.Helpers;
using BankingSystemAPI.Services.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
    

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    options =>
    {
        options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Enter JWT token: Bearer {your token}",
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
        });

        options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
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
    
//app_settings.json
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));
//Register DbContext
builder.Services.AddDbContext<BankingSystemDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();
// Register services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IEmailVerificationTokenService, EmailVerificationTokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();