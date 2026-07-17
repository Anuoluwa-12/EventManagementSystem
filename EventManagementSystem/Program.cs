using EventManagementSystem.Interface;
using EventManagementSystem.Service;

var builder = WebApplication.CreateBuilder(args);

/*
 * ==================================================
 * API BASE URL
 * ==================================================
 */

var apiBaseUrl =
    builder.Configuration["ApiSettings:BaseUrl"]
    ?? throw new InvalidOperationException(
        "ApiSettings:BaseUrl is missing from appsettings.json."
    );

/*
 * ==================================================
 * MVC
 * ==================================================
 */

builder.Services.AddControllersWithViews();

/*
 * ==================================================
 * SESSION
 * ==================================================
 */

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

/*
 * ==================================================
 * FRONTEND SERVICES
 * ==================================================
 */

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IDashboardService, DashboardService>();

builder.Services.AddScoped<IProfileService,ProfileService>();

builder.Services.AddScoped<IEventService, EventService>();

/*
 * ==================================================
 * ORGANIZER API SERVICE
 * ==================================================
 */

builder.Services.AddHttpClient<IOrganizerService, OrganizerService>
    (client =>
{
    client.BaseAddress =
        new Uri(apiBaseUrl);
});

/*
 * ==================================================
 * ADMIN DASHBOARD API SERVICE
 * ==================================================
 */

builder.Services.AddHttpClient<IAdminDashboardApiService, AdminDashboardApiService>
    (client =>
{
    client.BaseAddress =
        new Uri(apiBaseUrl);
});

var app = builder.Build();

/*
 * ==================================================
 * HTTP PIPELINE
 * ==================================================
 */

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(
        "/Home/Error"
    );

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

/*
 * ==================================================
 * ROUTES
 * ==================================================
 */

app.MapControllerRoute(
    name: "default",
    pattern:
        "{controller=Auth}/{action=Login}/{id?}"
);

app.Run();