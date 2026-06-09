using ConsolidatedService.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using MerchantCashFlow.Contracts;
using Microsoft.Extensions.Hosting;
using Serilog;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;

var builder = global::Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
    .UseSerilog((context, cfg) =>
    {
        cfg.Enrich.FromLogContext()
           .Enrich.WithEnvironmentName()
           .Enrich.WithThreadId()
           .WriteTo.Console()
           .WriteTo.File("logs/consolidated-.log", rollingInterval: Serilog.RollingInterval.Day);
    })
    .ConfigureServices((context, services) =>
    {
        var conn = context.Configuration.GetConnectionString("Postgres") ?? context.Configuration["ConnectionStrings:Postgres"];
        services.AddDbContext<ConsolidatedDbContext>(opt => opt.UseNpgsql(conn));

        // OpenTelemetry Tracing temporarily disabled here to avoid build-time extension mismatch in container.
        // It is configured for LaunchesService; we can reintroduce tracing here if needed after aligning packages.

        services.AddMassTransit(x =>
        {
            x.AddConsumer<LaunchesConsumer>();
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(context.Configuration["RabbitMQ:Host"] ?? context.Configuration["RabbitMQ__Host"] ?? "rabbitmq");
                cfg.ReceiveEndpoint("launches_queue", e =>
                {
                    e.ConfigureConsumer<LaunchesConsumer>(ctx);
                });
            });
        });

        services.AddHostedService<Worker>();
    });

var host = builder.Build();
await host.RunAsync();
