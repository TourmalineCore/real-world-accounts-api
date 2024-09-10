using System.Reflection;
using System.Runtime.InteropServices;
using Api;
using Api.Options;
using Application;
using Application.HttpClients;
using Application.Options;
using DataAccess;
using LoggingLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.EventLog;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using TourmalineCore.AspNetCore.JwtAuthentication.Core;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;

const string debugEnvironmentName = "Debug";
const string loggingSectionKey = "Logging";

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// configure ElasticSearch credentials
builder.Services.Configure<ElasticSearchOptions>(configuration.GetSection("ElasticSearchOptions"));
var elasticSearchOptions = configuration
    .GetSection(nameof(ElasticSearchOptions))
    .Get<ElasticSearchOptions>();

// add logging
builder.Services.AddScoped(_ => new LoggingAttribute("Accounts"));
ElkLogger.SetupLogger(
    elasticSearchOptions.ElasticSearchUrl,
    elasticSearchOptions.ElasticSearchLogin,
    elasticSearchOptions.ElasticSearchPassword);

builder.Services.AddControllers();

// add tracing
builder.Services.AddOpenTelemetry()
    .WithTracing(builder =>
    {
        builder
            .AddSource("Logs.Startup")
            .SetSampler(new AlwaysOnSampler())
            .SetResourceBuilder(
                ResourceBuilder
                    .CreateDefault()
                    .AddService("OpenTelemetry.RampUp.Accounts.*", serviceVersion: "0.0.1"))
            .AddAspNetCoreInstrumentation()
            .AddJaegerExporter(o =>
            {
                o.AgentHost = Environment.GetEnvironmentVariable("JAEGER_HOST") ?? "localhost";
                o.AgentPort = int.TryParse(Environment.GetEnvironmentVariable("JAEGER_PORT"), out var port) ? port : 6831;
            })

            .AddConsoleExporter();
    });
builder.Services.AddCors();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
        {
            var env = hostingContext.HostingEnvironment;
            var reloadOnChange = hostingContext.Configuration.GetValue("hostBuilder:reloadConfigOnChange", true);

            config.AddJsonFile("appsettings.json", true, reloadOnChange)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, reloadOnChange)
                .AddJsonFile("appsettings.Active.json", true, reloadOnChange);

            if (env.IsDevelopment() && !string.IsNullOrEmpty(env.ApplicationName))
            {
                var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                config.AddUserSecrets(appAssembly, true);
            }

            config.AddEnvironmentVariables();
            config.AddCommandLine(args);
        }
    );

var authenticationOptions = configuration.GetSection(nameof(AuthenticationOptions)).Get<AuthenticationOptions>();
builder.Services.AddJwtAuthentication(authenticationOptions).WithUserClaimsProvider<UserClaimsProvider>(UserClaimsProvider.PermissionClaimType);

builder.Services.AddPersistence(configuration);
builder.Services.AddApplication();

builder.Host.ConfigureLogging((hostingContext, logging) =>
        {
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            if (isWindows)
            {
                logging.AddFilter<EventLogLoggerProvider>(level => level >= LogLevel.Warning);
            }

            logging.AddConfiguration(hostingContext.Configuration.GetSection(loggingSectionKey));
            logging.AddConsole();
            logging.AddDebug();
            logging.AddEventSourceLogger();

            if (isWindows)
            {
                logging.AddEventLog();
            }

            logging.Configure(options =>
                    {
                        options.ActivityTrackingOptions = ActivityTrackingOptions.SpanId
                                                          | ActivityTrackingOptions.TraceId
                                                          | ActivityTrackingOptions.ParentId;
                    }
                );
        }
    );

builder.Services.Configure<HttpUrls>(configuration.GetSection(nameof(HttpUrls)));
builder.Services.Configure<AccountValidationOptions>(configuration.GetSection(nameof(AccountValidationOptions)));

var app = builder.Build();

app.UseCors(
        corsPolicyBuilder => corsPolicyBuilder
            .AllowAnyHeader()
            .SetIsOriginAllowed(_ => true)
            .AllowAnyMethod()
            .AllowAnyOrigin()
    );

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI();

using (var serviceScope = app.Services.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<AccountsDbContext>();
    await context.Database.MigrateAsync();
}

app.UseRouting();
app.UseJwtAuthentication();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
app.Run();