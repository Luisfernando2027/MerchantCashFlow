using LaunchesService.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// health checks
builder.Services.AddHealthChecks();

// prometheus metrics
builder.Services.AddHttpMetrics();

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
