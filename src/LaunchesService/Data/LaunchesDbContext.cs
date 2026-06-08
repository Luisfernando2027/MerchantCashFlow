using Microsoft.EntityFrameworkCore;
using LaunchesService.Models;

namespace LaunchesService.Data;

public class LaunchesDbContext : DbContext
{
    public LaunchesDbContext(DbContextOptions<LaunchesDbContext> options) : base(options) { }

    public DbSet<Launch> Launches { get; set; }
}
