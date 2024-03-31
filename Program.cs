
// Early init of NLog to allow startup and exception logging, before host is built
using NLog;
using NLog.Web;
using Prometheus;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllers();

    // healthchecks + prometheus
    // Export metrics from all HTTP clients registered in services
    builder.Services.UseHttpClientMetrics();
    builder.Services.AddHealthChecks().ForwardToPrometheus();


    // NLog: Setup NLog for Dependency injection
    builder.Host.UseNLog();

    var app = builder.Build();

    // Capture metrics about all received HTTP requests.
    app.UseHttpMetrics();

    app.UseAuthorization();
    // Enable the /metrics page to export Prometheus metrics.
    // Open http://localhost:5099/metrics to see the metrics.
    //
    // Metrics published in this sample:
    // * built-in process metrics giving basic information about the .NET runtime (enabled by default)
    // * metrics from .NET Event Counters (enabled by default, updated every 10 seconds)
    // * metrics from .NET Meters (enabled by default)
    // * metrics about requests made by registered HTTP clients used in SampleService (configured above)
    // * metrics about requests handled by the web app (configured above)
    // * ASP.NET health check statuses (configured above)
    // * custom business logic metrics published by the SampleService class
    app.MapMetrics();

    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}

