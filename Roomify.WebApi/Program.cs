using Roomify.Services;
using Roomify.WebApi;
using Roomify.WebApi.AuthorizationPolicies;
using Roomify.WebApi.Controllers;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using Roomify.Commons.Extensions;
using Roomify.Commons.Services;
using Roomify.Extensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Sentry automatically loads appsettings.json and environment variables, binding to SentryAspNetCoreOptions
// Don't forget to set TracesSampleRate to lower values (for example, 0.2) in Production environment!
builder.WebHost.UseSentry();
builder.Host.ConfigureSerilogWithSentry(options =>
{
    options.WriteErrorLogsToFile = "/Logs/Roomify.WebApi.log";
    options.WriteToSentry = true;
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddRazorPages();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen();

builder.Services.Configure<AppSettings>(builder.Configuration);
builder.Services.Configure<OpenIdValidationOptions>(builder.Configuration.GetSection("OpenIdConnect"));
builder.Services.Configure<MinIoOptions>(configuration.GetSection("MinIO"));
builder.Services.AddTransient<IQRCodeGeneratorService, QRCodeGeneratorService>();


builder.Services.AddApplicationServices(options =>
{
    options.PostgreSqlConnectionString = configuration.GetConnectionString("PostgreSql");
    options.AddWebAppOnlyServices = true;
});
builder.Services.AddMinIoService(options =>
{
    options.EndPoint = configuration["MinIO:EndPoint"];
    options.AccessKey = configuration["MinIO:AccessKey"];
    options.ServerKey = configuration["MinIO:ServerKey"];
    options.BucketName = configuration["MinIO:BucketName"];
    options.IsUseSsl = configuration.GetValue<bool>("MinIO:IsUseSsl");
});
builder.Services.AddTransient<IStorageService, StorageService>();

builder.Services.AddAutoMapper(typeof(UserController.UpdateUserApiModelAutoMapper));

builder.Services.AddOpenIdConnectServer(options =>
{
    // Use api/generate-rsa-keys to get new random values 
    options.SigningKey = configuration["oidcSigningKey"];
    options.EncryptionKey = configuration["oidcEncryptionKey"];
});
builder.Services.AddEntityFrameworkCoreAutomaticMigrations();

builder.Services.AddAuthorization(options =>
{
    foreach (var policy in AuthorizationPolicyMap.Map)
    {
        options.AddPolicy(policy.Key, policy.Value);
    }
    // Set fallback policy to apply authorization policy to all unprotected API
    // options.FallbackPolicy = AuthorizationPolicyMap.Map[AuthorizationPolicyNames.ScopeApi];
});


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseForwardedHeaders();

app.MapHealthChecks("/healthz", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseExceptionHandler("/error");

app.UseStaticFiles();

// Write streamlined request completion events, instead of the more verbose ones from the framework.
// To use the default framework request logging instead, remove this line and set the "Microsoft"
// level in appsettings.json to "Information".
app.UseSerilogRequestLogging();

app.UseRouting();

// https://docs.sentry.io/platforms/dotnet/guides/aspnetcore/performance/instrumentation/automatic-instrumentation
app.UseSentryTracing();

app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Starting web host");
    app.Run();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}
