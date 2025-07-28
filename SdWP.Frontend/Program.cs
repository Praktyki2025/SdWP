using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SdWP.Data.Context;
using SdWP.Data.Models;
using SdWP.Data.Repositories;
using SdWP.Frontend.Components;
using SdWP.Service.IServices;
using SdWP.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddBlazorBootstrap();

builder.Services.AddControllers();
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5267/");
});

// Konfiguracja Identity z InMemoryUserRepository
builder.Services.AddIdentityCore<User>(options =>
{
    options.User.RequireUniqueEmail = true;

    // Konfiguracja has³a - dostosuj do swoich potrzeb
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false; // Pozwól na has³a bez znaków specjalnych
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    // Konfiguracja lockout
    options.Lockout.AllowedForNewUsers = false;
})
.AddUserStore<InMemoryUserRepository>()
.AddDefaultTokenProviders();

// Dodaj SignInManager
builder.Services.AddScoped<SignInManager<User>>();

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-XSRF-TOKEN";
});

// Register services
builder.Services.AddScoped<IUserLoginService, UserLoginServices>();
builder.Services.AddScoped<IUserRegisterService, UserRegisterService>();

builder.Services.AddScoped<ProjectRepository>();
builder.Services.AddScoped<IProjectInteractionsService, ProjectInteractionsService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

//app.Use(async (context, next) =>
//{
//    var anti = context.RequestServices.GetRequiredService<IAntiforgery>();

//    if (context.Request.Path.StartsWithSegments("/api") &&
//        (HttpMethods.IsPost(context.Request.Method) ||
//         HttpMethods.IsPut(context.Request.Method) ||
//         HttpMethods.IsDelete(context.Request.Method)))
//    {
//        await anti.ValidateRequestAsync(context);
//    }

//    await next();
//});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapControllers();

app.Run();