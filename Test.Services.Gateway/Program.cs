using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Test.Services.Gateway.DelegatingHandlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAccessTokenManagement();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var authenticationScheme = "TestGatewayAuthenticationScheme";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(authenticationScheme, options =>
    {
        options.Authority = "https://localhost:5010";
        options.Audience = "testgateway";
    });

builder.Services.AddHttpClient();

builder.Services.AddScoped<TokenExchangeDelegatingHandler>();

builder.WebHost.UseKestrel();

builder.WebHost.UseContentRoot(Directory.GetCurrentDirectory());

builder.WebHost.ConfigureAppConfiguration((hostingContext, config) =>
{
    config
        .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
        .AddJsonFile("appsettings.json", true, true)
        .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
        .AddJsonFile("ocelot.json")
        .AddEnvironmentVariables();
}).ConfigureServices(s =>
{
    s.AddOcelot().AddDelegatingHandler<TokenExchangeDelegatingHandler>();
})
.ConfigureLogging((hostingContext, logging) =>
{
    // logger
})
.UseIISIntegration();

//builder.Services.AddOcelot();
var app = builder.Build();

app.UseOcelot().Wait();
app.Run();