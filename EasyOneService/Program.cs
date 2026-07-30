using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Textile.Core.DI;
using EasyOneService.ErrorHandlig;

var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;

builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddControllers();


builder.Services.AddUnitOfWork();

builder.Services.AddTextileDb(Configuration);

// CORS ---------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("TextileCors",
        policy =>
        {
            policy.SetIsOriginAllowed(_ => true) // allow all
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});
// ---------------------------------------------------------


// ----------------------------
//  ADD JWT AUTHENTICATION
// ----------------------------
// ----------------------------
//  ADD JWT AUTHENTICATION
// ----------------------------
var jwtSettings = Configuration.GetSection("JwtSettings");

// We use ONE key + issuer (shared)
var key = Encoding.UTF8.GetBytes(jwtSettings["Web:Key"]);
var issuer = jwtSettings["Web:Issuer"];

// All valid audiences
var validAudiences = new[]
{
    jwtSettings["Web:Audience"],
    jwtSettings["Mobile:Audience"],
    jwtSettings["Windows:Audience"]
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

        ValidIssuer = issuer,
        ValidAudiences = validAudiences,

        IssuerSigningKey = new SymmetricSecurityKey(key),
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

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseCors("TextileCors");
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
