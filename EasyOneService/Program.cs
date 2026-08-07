using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Textile.Core.DI;
using EasyOneService.ErrorHandlig;

var builder = WebApplication.CreateBuilder(args);
const string CorsPolicyName = "AngularClient";
var Configuration = builder.Configuration;

builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddControllers();


builder.Services.AddUnitOfWork();

builder.Services.AddTextileDb(Configuration);

// CORS ---------------------------------------------------
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

var allowedOriginsFromEnvironment = (builder.Configuration["CORS_ALLOWED_ORIGINS"] ?? string.Empty)
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

allowedOrigins = allowedOrigins
    .Concat(allowedOriginsFromEnvironment)
    .Select(NormalizeOrigin)
    .OfType<string>()
    .Where(origin => !string.IsNullOrWhiteSpace(origin))
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToArray();

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
// ---------------------------------------------------------


var jwtKey = GetRequiredConfigurationValue(
    Configuration,
    "JwtSettings:Web:Key",
    "JWT_WEB_KEY");

var jwtIssuer = GetRequiredConfigurationValue(
    Configuration,
    "JwtSettings:Web:Issuer",
    "JWT_WEB_ISSUER");

var validAudiences = new[]
{
    GetRequiredConfigurationValue(Configuration, "JwtSettings:Web:Audience", "JWT_WEB_AUDIENCE"),
    GetRequiredConfigurationValue(Configuration, "JwtSettings:Mobile:Audience", "JWT_MOBILE_AUDIENCE"),
    GetRequiredConfigurationValue(Configuration, "JwtSettings:Windows:Audience", "JWT_WINDOWS_AUDIENCE")
};

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

        ValidIssuer = jwtIssuer,
        ValidAudiences = validAudiences,

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };
});


// Authorization
builder.Services.AddAuthorization();


// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Textile API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token: **Bearer {your token}**"
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement()
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
            new string[] {}
        }
    };

    c.AddSecurityRequirement(securityRequirement);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(CorsPolicyName);
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

static string GetRequiredConfigurationValue(IConfiguration configuration, params string[] keys)
{
    foreach (var key in keys)
    {
        var value = configuration[key];

        if (!string.IsNullOrWhiteSpace(value))
        {
            return value;
        }
    }

    throw new InvalidOperationException($"Missing required configuration value. Set one of: {string.Join(", ", keys)}");
}

static string? NormalizeOrigin(string origin)
{
    origin = origin.Trim().TrimEnd('/');

    return Uri.TryCreate(origin, UriKind.Absolute, out var uri)
        ? uri.GetLeftPart(UriPartial.Authority)
        : origin;
}
