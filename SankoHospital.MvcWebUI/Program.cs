var builder = WebApplication.CreateBuilder(args);

// MVC hizmetleri
builder.Services.AddControllersWithViews();

// Session, HttpClient vb.
builder.Services.AddSession();
builder.Services.AddHttpClient();

var app = builder.Build();
app.UseSession();

// Orta katmanlar
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Yalnızca Conventional Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();