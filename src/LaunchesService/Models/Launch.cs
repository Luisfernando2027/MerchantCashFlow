using System.ComponentModel.DataAnnotations;

namespace LaunchesService.Models;

public class Launch
{
    [Key]
    public Guid Id { get; set; }
    public Guid MerchantId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "BRL";
    public DateTime OccurredAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
