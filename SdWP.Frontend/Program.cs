using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SdWP.Data.Context;
using SdWP.Data.IData;
using SdWP.Data.Models;
using SdWP.Data.Repositories;
using SdWP.Frontend.Components;
using SdWP.Frontend.Functions;
using SdWP.Service.Helpers;
using SdWP.Service.IServices;
using SdWP.Service.Services;
using SdWP.Service.Services.Mailing;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazorBootstrap();

builder.Services.AddControllers();
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5267/");
});

builder.Services.AddAuthorizationCore();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Lockout.AllowedForNewUsers = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/access-denied";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.None
        : CookieSecurePolicy.Always;
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILoginService, LoginServices>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ErrorLogRepository>();
builder.Services.AddScoped<IErrorLogHelper, ErrorLogHelper>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<SendLogToDatabase>();
builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectService, ProjectService>();

builder.Services.AddScoped<IValuationRepository, ValuationRepository>();
builder.Services.AddScoped<IValuationService, ValuationService>();
builder.Services.AddScoped<ICostTypeRepository, CostTypeRepository>();
builder.Services.AddScoped<ICostCategoryRepsoitory, CostCategoryRepsoitory>();
builder.Services.AddScoped<IUserGroupTypeRepository, UserGroupTypeRepository>();

builder.Services.AddScoped<IValuationItemRepository, ValuationItemRepository>();
builder.Services.AddScoped<IValuationItemService, ValuationItemService>();

builder.Services.AddScoped<IValuationItemRepository, ValuationItemRepository>();
builder.Services.AddScoped<IValuationItemService, ValuationItemService>();

builder.Services.AddScoped<ILinkRepository, LinkRepository>();
builder.Services.AddScoped<ILinkServices, LinkServices>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpContextAccessor();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<EmailService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();

        await SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database initialization.");
    }
}

// testowy mail, zostawam aby wiedzieæ ¿e dzia³a
using (var scope = app.Services.CreateScope())
{
    try
    {
        var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();
        await emailService.SendTestEmailAsync("user@example.pl");
    }
    catch (Exception e)
    {
        Log.Error(e.Message);
    }
}


app.UseStaticFiles();

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapControllers();

app.Run();
