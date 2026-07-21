using EventManagementSystem.Interface;
using EventManagementSystem.Service;

var builder = WebApplication.CreateBuilder(args);

/*
 * ====================
 * API BASE URL
 * ====================
 */

var apiBaseUrl =
    builder.Configuration["ApiSettings:BaseUrl"]
    ?? throw new InvalidOperationException(
        "ApiSettings:BaseUrl is missing from appsettings.json."
    );

/*
 * =================
 * MVC
 * =================
 */

builder.Services.AddControllersWithViews();

/*
 * =================
 * SESSION
 * =================
 */

builder.Services.AddSession(options =>
{
    options.IdleTimeout =
        TimeSpan.FromHours(2);

    options.Cookie.HttpOnly = true;

    options.Cookie.IsEssential = true;

    options.Cookie.SameSite =
        SameSiteMode.Lax;
});

builder.Services.AddHttpContextAccessor();

/*
 * =================================
 * STANDARD FRONTEND SERVICES
 * =================================
 */

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IDashboardService,DashboardService>();

builder.Services.AddScoped<IProfileService, ProfileService>();

builder.Services.AddScoped<IEventService, EventService>();

builder.Services.AddHttpClient<
    ICheckoutService,
    CheckoutService
>(client =>
{
    client.BaseAddress =
        new Uri(apiBaseUrl);
});
/*
 * Do not register these in the frontend:
 *
 * IAdminDashboardService
 * AdminDashboardService
 *
 * They belong to EventManagement.API.
 */

/*
 * ==================================================
 * ORGANIZER API SERVICE
 * ==================================================
 */

builder.Services.AddHttpClient<IOrganizerService, OrganizerService
>(client =>
{
    client.BaseAddress =
        new Uri(apiBaseUrl);
});

/*
 * ==================================================
 * ADMIN DASHBOARD API SERVICE
 * ==================================================
 */

builder.Services.AddHttpClient<IAdminDashboardApiService, AdminDashboardApiService
>(client =>
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

app.UseStaticFiles();

app.UseRouting();

/*
 * Session must run before controllers
 * read the Role, UserId or token.
 */
app.UseSession();

app.UseAuthorization();

/*
 * Keep this only if the project is using
 * the .NET static asset mapping feature.
 */
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