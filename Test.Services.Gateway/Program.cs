using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel()
    .UseContentRoot(Directory.GetCurrentDirectory())
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config
            .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
            .AddJsonFile("ocelot.json")
            .AddEnvironmentVariables();
    }).ConfigureServices(s =>
    {
        s.AddOcelot();
    })
    .ConfigureLogging((hostingContext, logging) =>
    {
        // logger
    })
    .UseIISIntegration();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var authenticationScheme = "TestGatewayAuthenticationScheme";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(authenticationScheme, options =>
    {
        options.Authority = "https://localhost:5010";
        options.Audience = "testgateway";
    });

//builder.Services.AddOcelot();
var app = builder.Build();

app.UseOcelot().Wait();
app.Run();