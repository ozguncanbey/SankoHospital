using SankoHospital.MvcWebUI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// MVC hizmetleri
builder.Services.AddControllersWithViews();

// Session, HttpClient vb.
builder.Services.AddSession();
builder.Services.AddHttpClient();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });
builder.Services.AddCustomServices();

var app = builder.Build();
app.UseSession();

// Orta katmanlar
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession(); // UseRouting'den sonra, UseEndpoints veya MapControllerRoute'dan önce ekleyin.
app.UseAuthentication();
app.UseAuthorization();

// Yalnızca Conventional Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();