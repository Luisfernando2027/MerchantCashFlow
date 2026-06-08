using LaunchesService.Data;
using LaunchesService.Messaging;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LaunchesDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
