using System.Text;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using LoyaltyAPI.Models;
using LoyaltyAPI.Services;
using MySqlConnector;
using System.Data;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// ✅ Load .env
DotNetEnv.Env.Load();

// ✅ Set environment variables to Configuration
builder.Configuration["ConnectionStrings:LoyaltyDbConnection"] = Environment.GetEnvironmentVariable("LOYALTY_DB");
builder.Configuration["ConnectionStrings:WebLoanAppDB"] = Environment.GetEnvironmentVariable("WEBLOAN_DB");
builder.Configuration["ConnectionStrings:CobxDavaoDB"] = Environment.GetEnvironmentVariable("COBX_DAVAO_DB");
builder.Configuration["ConnectionStrings:NFCConnection"] = Environment.GetEnvironmentVariable("NFC_DB");  // Fixed this line

builder.Configuration["JwtSettings:SecretKey"] = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
builder.Configuration["JwtSettings:Issuer"] = Environment.GetEnvironmentVariable("JWT_ISSUER");
builder.Configuration["JwtSettings:Audience"] = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

builder.Configuration["ApiSettings:ApiKey"] = Environment.GetEnvironmentVariable("API_KEY");
builder.Configuration["SwaggerSettings:ApiKey"] = Environment.GetEnvironmentVariable("API_KEY");

// ✅ Extract JWT settings
var jwtSecretKey = builder.Configuration["JwtSettings:SecretKey"];
var jwtIssuer = builder.Configuration["JwtSettings:Issuer"];
var jwtAudience = builder.Configuration["JwtSettings:Audience"];

if (string.IsNullOrEmpty(jwtSecretKey))
    throw new InvalidOperationException("JWT Secret Key not found.");

// ✅ Connection strings
var loyaltyDbConnection = builder.Configuration.GetConnectionString("LoyaltyDbConnection")
    ?? throw new InvalidOperationException("LoyaltyDbConnection not found.");
var webLoanAppConnection = builder.Configuration.GetConnectionString("WebLoanAppDB")
    ?? throw new InvalidOperationException("WebLoanAppDB not found.");
var cobxDavaoConnection = builder.Configuration.GetConnectionString("CobxDavaoDB")
    ?? throw new InvalidOperationException("CobxDavaoDB not found.");
var nfcDbConnection = builder.Configuration.GetConnectionString("NFCConnection")
    ?? throw new InvalidOperationException("NFCConnection not found.");  // Fixed this line

// ✅ EF Core DbContexts
builder.Services.AddDbContextFactory<LoyaltyDbContext>(options =>
    options.UseMySql(loyaltyDbConnection, ServerVersion.AutoDetect(loyaltyDbConnection),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure())
        .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information));

builder.Services.AddDbContext<WebLoanAppDbContext>(options =>
    options.UseMySql(webLoanAppConnection, new MySqlServerVersion(new Version(8, 0, 21))));

builder.Services.AddDbContext<COBXDbContext>(options =>
    options.UseSqlServer(cobxDavaoConnection)
        .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information));

builder.Services.AddDbContext<NFCContext>(options =>  // Added NFCContext registration
    options.UseMySql(nfcDbConnection, new MySqlServerVersion(new Version(8, 0, 21)))
        .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information));

// ✅ Register Services
builder.Services.AddScoped<IDbConnection>(_ => new MySqlConnection(webLoanAppConnection));
builder.Services.AddSingleton<DataService>(_ => new DataService(webLoanAppConnection, cobxDavaoConnection));
builder.Services.AddScoped<WebLoanAppService>();
builder.Services.AddScoped<COBXService>();
builder.Services.AddScoped<TransactionService>();
builder.Services.AddScoped<ReferralService>();
builder.Services.AddScoped<ActivityService>();
builder.Services.AddScoped<LoyaltyService>();
builder.Services.AddScoped<NfcTransactionService>();

// ✅ CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// ✅ Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// ✅ JWT Authentication
var key = Encoding.ASCII.GetBytes(jwtSecretKey);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ✅ MVC & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
});

// ✅ App Pipeline
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
