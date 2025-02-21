using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SankoHospital.WebApi.Extensions; // Örn. AddCustomServices gibi

var builder = WebApplication.CreateBuilder(args);

// 1. Controller ve JSON Ayarları
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Enum'ları string olarak saklamak isterseniz
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// 2. Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. Custom Services (ör. UserService, ITokenService, vs.)
builder.Services.AddCustomServices(); // Bu sizin extension method'unuz

// 4. JWT Authentication
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // Burada secret key vb. tanımlıyoruz
        var secretKey = "BuCokGucluBirGizliAnahtar!123456789"; // Daha uzun ve güvenli yapın
        var key = Encoding.UTF8.GetBytes(secretKey);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = "sankohospital.com",
            ValidateAudience = true,
            ValidAudience = "sankohospital.com",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero // Token süresini tam dikkate al
        };
    });

// 5. Authorization
builder.Services.AddAuthorization();

var app = builder.Build();

// 6. Swagger + Dev Check
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 7. Middleware Pipeline
app.UseHttpsRedirection();

// Sıralama önemli: Önce Authentication, sonra Authorization
app.UseAuthentication();
app.UseAuthorization();

// 8. Map Controllers
app.MapControllers();

app.Run();