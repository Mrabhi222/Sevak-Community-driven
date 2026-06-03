using Sevak.Infrastructure.Data;
using Sevak.Application.Interfaces;
using Sevak.Infrastructure.Services;
using Sevak.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

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
            RoleClaimType = "role"
        };
    });

// Register Services
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();


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
app.UseCors("AllowReact");
app.MapScalarApiReference();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();