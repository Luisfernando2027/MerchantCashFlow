using LaunchesService.Data;
using LaunchesService.Models;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using MerchantCashFlow.Contracts;

namespace LaunchesService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LaunchesController : ControllerBase
{
    private readonly LaunchesDbContext _db;
    private readonly IPublishEndpoint _publishEndpoint;

    public LaunchesController(LaunchesDbContext db, IPublishEndpoint publishEndpoint)
    {
        _db = db;
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Launch input)
    {
        input.Id = Guid.NewGuid();
        input.CreatedAt = DateTime.UtcNow;
        _db.Launches.Add(input);
        await _db.SaveChangesAsync();

        await _publishEndpoint.Publish(new LaunchCreated
        {
            Id = input.Id,
            MerchantId = input.MerchantId,
            Amount = input.Amount,
            OccurredAt = input.OccurredAt
        });

        return CreatedAtAction(nameof(Get), new { id = input.Id }, input);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var item = await _db.Launches.FindAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }
}
