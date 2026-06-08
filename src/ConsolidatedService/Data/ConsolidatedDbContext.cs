using Microsoft.EntityFrameworkCore;
using ConsolidatedService.Models;

namespace ConsolidatedService.Data;

public class ConsolidatedDbContext : DbContext
{
    public ConsolidatedDbContext(DbContextOptions<ConsolidatedDbContext> options) : base(options) { }

    public DbSet<Consolidated> Consolidateds { get; set; }
    public DbSet<ProcessedEvent> ProcessedEvents { get; set; }
}
