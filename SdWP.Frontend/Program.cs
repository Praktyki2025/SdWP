using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using SdWP.Data.Models;
using SdWP.Data.Repositories;
using SdWP.Frontend.Components;
using SdWP.Service.IServices;
using SdWP.Service.Services;
using Microsoft.AspNetCore.Components.Server;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Lockout.AllowedForNewUsers = false;
})
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
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
});

builder.Services.AddBlazorBootstrap();
builder.Services.AddSingleton<InMemoryUserRepository>();
builder.Services.AddSingleton<InMemoryRoleRepository>();
builder.Services.AddSingleton<IUserStore<User>>(provider => provider.GetService<InMemoryUserRepository>()!);
builder.Services.AddSingleton<IUserRoleStore<User>>(provider => provider.GetService<InMemoryUserRepository>()!); 
builder.Services.AddSingleton<IUserPasswordStore<User>>(provider => provider.GetService<InMemoryUserRepository>()!); 
builder.Services.AddSingleton<IUserEmailStore<User>>(provider => provider.GetService<InMemoryUserRepository>()!); 
builder.Services.AddSingleton<IRoleStore<IdentityRole>>(provider => provider.GetService<InMemoryRoleRepository>()!);

builder.Services.AddScoped<SignInManager<User>>();
builder.Services.AddScoped<RoleManager<IdentityRole>>();
builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
builder.Services.AddScoped<IUserRegisterService, UserRegisterService>();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-XSRF-TOKEN";
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IUserLoginService, UserLoginServices>();

builder.Services.AddAuthorizationBuilder();


builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BaseAddress"] ?? "http://localhost:5267");
});
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration["BaseAddress"] ?? "http://localhost:5267")
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapControllers();

app.Run();