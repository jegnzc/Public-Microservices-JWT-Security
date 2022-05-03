using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Test.Services.APITest2.Services;
using Test.Services.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor();

var requireAuthenticatedUserPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

builder.Services.AddControllersWithViews(options =>
    options.Filters.Add(new AuthorizeFilter(requireAuthenticatedUserPolicy)));

builder.Services.AddHttpClient<ITestService, TestService>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["ApiConfigs:APITest:Uri"]);
});

builder.Services.AddHttpClient<ITestService2, TestService2>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["ApiConfigs:APITest2:Uri"]);
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.Authority = "https://localhost:5010/";
        options.ClientId = "test";
        options.ResponseType = "code";
        options.SaveTokens = true;
        options.ClientSecret = "test.secret";
        options.GetClaimsFromUserInfoEndpoint = true;
        options.Scope.Add("test2.fullaccess");
        options.Scope.Add("test1.fullaccess");
        options.Scope.Add("testgateway.fullaccess");
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();