using LaunchesService.Data;

using MassTransit;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console());

builder.Services.AddControllers();
builder.Services.AddHealthChecks();

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
app.MapControllers();
app.MapHealthChecks("/health", new HealthCheckOptions { Predicate = _ => true });

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LaunchesDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
