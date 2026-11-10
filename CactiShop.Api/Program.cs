using System.Text;
using CactiShop.Api.Data;
using CactiShop.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException(
        "Не задана строка подключения ConnectionStrings:DefaultConnection. " +
        "Для разработки: dotnet user-secrets set \"ConnectionStrings:DefaultConnection\" \"...\" (см. CONTRIBUTING.md).");

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || Encoding.UTF8.GetByteCount(jwtKey) < 32)
    throw new InvalidOperationException(
        "Jwt:Key не задан или короче 32 байт. " +
        "Для разработки: dotnet user-secrets set \"Jwt:Key\" \"...\" (см. CONTRIBUTING.md).");

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<TokenService>();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;

    if (response.StatusCode == StatusCodes.Status404NotFound)
    {
        response.ContentType = "application/json";

        var path = context.HttpContext.Request.Path;

        var result = new
        {
            message = $"Эндпоинт {path} не существует."
        };

        await response.WriteAsJsonAsync(result);
    }
});

app.Run();
