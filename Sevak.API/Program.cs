using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Sevak.API.Middleware;
using Sevak.Application.Interfaces;
using Sevak.Infrastructure.AI;
using Sevak.Infrastructure.Data;
using Sevak.Infrastructure.Repositories;
using Sevak.Infrastructure.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Get Ollama settings
var ollamaSettings = new OllamaSettings();
builder.Configuration.GetSection("AiSettings").Bind(ollamaSettings);
builder.Services.AddSingleton(ollamaSettings);

// Add HttpClient for Ollama
builder.Services.AddHttpClient<OllamaApiClient>()
    .ConfigureHttpClient(client =>
    {
        client.BaseAddress = new Uri(ollamaSettings.OllamaBaseUrl);
        client.Timeout = TimeSpan.FromMinutes(5);
    });

// PostgreSQL Database
builder.Services.AddDbContext<SevakDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => npgsqlOptions.EnableRetryOnFailure()
    ));

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:SecretKey"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            RoleClaimType = "role",
            NameClaimType = "sub"
        };
        options.MapInboundClaims = false;
    });

// Register Services
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<EventRecommendationAgent>();


builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", builder =>
        builder.WithOrigins("http://localhost:3000", "http://localhost:3001")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials());
});

var app = builder.Build();

// Auto-migrate on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SevakDbContext>();
    db.Database.Migrate();
}

app.MapOpenApi();
app.MapScalarApiReference();
app.UseHttpsRedirection();
app.UseCors("AllowReact");
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();