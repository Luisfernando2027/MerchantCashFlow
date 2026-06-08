using ConsolidatedService.Data;
using ConsolidatedService.Models;
using MassTransit;
using MerchantCashFlow.Contracts;
using Microsoft.EntityFrameworkCore;

public class LaunchesConsumer : IConsumer<LaunchCreated>
{
    private readonly ConsolidatedDbContext _db;

    public LaunchesConsumer(ConsolidatedDbContext db)
    {
        _db = db;
    }

    public async Task Consume(ConsumeContext<LaunchCreated> context)
    {
        var msg = context.Message;
        // idempotency: check if event was already processed
        var processed = await _db.ProcessedEvents.FindAsync(msg.Id);
        if (processed != null) return; // already processed
        var date = msg.OccurredAt.Date;
        var existing = await _db.Consolidateds.FirstOrDefaultAsync(x => x.MerchantId == msg.MerchantId && x.Date == date);
        if (existing == null)
        {
            existing = new Consolidated { Id = Guid.NewGuid(), MerchantId = msg.MerchantId, Date = date, Balance = msg.Amount };
            _db.Consolidateds.Add(existing);
        }
        else
        {
            existing.Balance += msg.Amount;
            _db.Consolidateds.Update(existing);
        }
        _db.ProcessedEvents.Add(new ProcessedEvent { EventId = msg.Id, ProcessedAt = DateTime.UtcNow });
        await _db.SaveChangesAsync();
    }
}
