using ConsolidatedService.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using MerchantCashFlow.Contracts;
using Serilog;
using Microsoft.Extensions.Hosting;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
    .UseSerilog((ctx, lc) => lc.WriteTo.Console())
    .ConfigureServices((HostBuilderContext context, IServiceCollection services) =>
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

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ConsolidatedDbContext>();
    db.Database.EnsureCreated();
}

await host.RunAsync();


