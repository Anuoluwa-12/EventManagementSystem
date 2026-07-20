using EventManagement.API.Data;
using EventManagement.API.Interface;
using EventManagement.API.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

/*
 * ==================================================
 * CONTROLLERS
 * ==================================================
 */

builder.Services.AddControllers();

/*
 * ==================================================
 * APPLICATION SERVICES
 * ==================================================
 */

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IOrganizerService, OrganizerService>();
builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();

// Add this only when the service has been implemented.
// builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();

/*
 * ==================================================
 * DATABASE
 * ==================================================
 */

var connectionString =
    builder.Configuration.GetConnectionString(
        "DefaultConnection"
    );

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "The DefaultConnection connection string is missing."
    );
}

builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
    {
        options.UseSqlServer(connectionString);
    }
);

/*
 * ==================================================
 * JWT AUTHENTICATION
 * ==================================================
 */

var jwtKey =
    builder.Configuration["Jwt:Key"];

var jwtIssuer =
    builder.Configuration["Jwt:Issuer"];

var jwtAudience =
    builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "Jwt:Key is missing from appsettings.json."
    );
}

if (string.IsNullOrWhiteSpace(jwtIssuer))
{
    throw new InvalidOperationException(
        "Jwt:Issuer is missing from appsettings.json."
    );
}

if (string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException(
        "Jwt:Audience is missing from appsettings.json."
    );
}

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,

                ValidateAudience = true,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtIssuer,

                ValidAudience = jwtAudience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)
                    ),

                NameClaimType =
                    ClaimTypes.NameIdentifier,

                RoleClaimType =
                    ClaimTypes.Role,

                ClockSkew =
                    TimeSpan.Zero
            };
    });

builder.Services.AddAuthorization();

/*
 * ==================================================
 * SWAGGER
 * ==================================================
 */

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "EventManagement API",

            Version = "v1",

            Description =
                "API for managing users, organizers, events and bookings."
        }
    );

    /*
     * Adds JWT Bearer authentication
     * to the Swagger interface.
     *
     * This creates the Authorize button.
     */
    options.AddSecurityDefinition(
        "bearer",
        new OpenApiSecurityScheme
        {
            Type =
                SecuritySchemeType.Http,

            Scheme =
                "bearer",

            BearerFormat =
                "JWT",

            In =
                ParameterLocation.Header,

            Name =
                "Authorization",

            Description =
                "Paste your JWT token here. Do not include the word Bearer."
        }
    );

    /*
     * Swashbuckle 10 requires the generated
     * OpenApiDocument when creating a reference.
     */
    options.AddSecurityRequirement(
        document =>
            new OpenApiSecurityRequirement
            {
                [
                    new OpenApiSecuritySchemeReference(
                        "bearer",
                        document
                    )
                ] = []
            }
    );
});

/*
 * ==================================================
 * CORS
 * ==================================================
 */

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        }
    );
});

/*
 * ==================================================
 * SESSION
 * ==================================================
 */

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout =
        TimeSpan.FromMinutes(30);

    options.Cookie.HttpOnly =
        true;

    options.Cookie.IsEssential =
        true;
});

builder.Services.AddHttpContextAccessor();

/*
 * ==================================================
 * BUILD APPLICATION
 * ==================================================
 */

var app = builder.Build();

/*
 * ==================================================
 * SWAGGER PIPELINE
 * ==================================================
 */

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "EventManagement API v1"
        );

        options.RoutePrefix =
            "swagger";

        options.DocumentTitle =
            "EventManagement API Documentation";
    });
}

/*
 * ==================================================
 * HTTP PIPELINE
 * ==================================================
 */

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("AllowAll");

app.UseSession();

/*
 * Authentication must come before authorization.
 */
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();