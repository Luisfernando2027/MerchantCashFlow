using LaunchesService.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using Serilog;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((context, cfg) =>
{
    cfg.Enrich.FromLogContext()
       .WriteTo.Console();
});

// OpenTelemetry Tracing
builder.Services.AddOpenTelemetryTracing(otel =>
{
    otel.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("LaunchesService"))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSource("MassTransit")
        .AddZipkinExporter(options =>
        {
            options.Endpoint = new Uri(builder.Configuration["Tracing:ZipkinUrl"] ?? "http://zipkin:9411/api/v2/spans");
        });
});

builder.Services.AddControllers();
// health checks
builder.Services.AddHealthChecks();

// prometheus metrics (AddHttpMetrics removed to avoid build-time extension lookup in container)

var conn = builder.Configuration.GetConnectionString("Postgres") ?? builder.Configuration["ConnectionStrings:Postgres"];
builder.Services.AddDbContext<LaunchesDbContext>(opt => opt.UseNpgsql(conn));

// MassTransit RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? builder.Configuration["RabbitMQ__Host"] ?? "rabbitmq");
    });
});

var app = builder.Build();
app.MapHealthChecks("/health");
app.UseHttpMetrics();
app.MapMetrics("/metrics");
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LaunchesDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
