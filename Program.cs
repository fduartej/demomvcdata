using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using demomvcdata.Data;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var redisConfiguration = builder.Configuration["Redis:Configuration"];
if (!string.IsNullOrWhiteSpace(redisConfiguration))
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.ConfigurationOptions = BuildRedisConfigurationOptions(redisConfiguration);
    });
}

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Configurar Session State
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de expiración de la sesión
    options.Cookie.HttpOnly = true; // Protección XSS
    options.Cookie.IsEssential = true; // Necesario para GDPR
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession(); // Habilitar Session State

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();

static ConfigurationOptions BuildRedisConfigurationOptions(string redisConfiguration)
{
    if (Uri.TryCreate(redisConfiguration, UriKind.Absolute, out var redisUri) &&
        (redisUri.Scheme.Equals("redis", StringComparison.OrdinalIgnoreCase) ||
         redisUri.Scheme.Equals("rediss", StringComparison.OrdinalIgnoreCase)))
    {
        var options = new ConfigurationOptions
        {
            AbortOnConnectFail = false,
            ConnectRetry = 2,
            ConnectTimeout = 5000,
            AsyncTimeout = 5000,
            Ssl = redisUri.Scheme.Equals("rediss", StringComparison.OrdinalIgnoreCase)
        };

        options.EndPoints.Add(redisUri.Host, redisUri.Port > 0 ? redisUri.Port : 6379);

        if (!string.IsNullOrWhiteSpace(redisUri.UserInfo))
        {
            var userInfoParts = redisUri.UserInfo.Split(':', 2);
            options.User = Uri.UnescapeDataString(userInfoParts[0]);

            if (userInfoParts.Length == 2)
            {
                options.Password = Uri.UnescapeDataString(userInfoParts[1]);
            }
        }

        return options;
    }

    return ConfigurationOptions.Parse(redisConfiguration);
}
