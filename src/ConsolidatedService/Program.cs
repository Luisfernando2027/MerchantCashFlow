using ConsolidatedService.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using MerchantCashFlow.Contracts;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var conn = context.Configuration.GetConnectionString("Postgres") ?? context.Configuration["ConnectionStrings:Postgres"];
        services.AddDbContext<ConsolidatedDbContext>(opt => opt.UseNpgsql(conn));

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
    }).Build();

await builder.RunAsync();
