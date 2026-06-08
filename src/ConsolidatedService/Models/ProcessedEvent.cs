using System.ComponentModel.DataAnnotations;

namespace ConsolidatedService.Models;

public class ProcessedEvent
{
    [Key]
    public Guid EventId { get; set; }
    public DateTime ProcessedAt { get; set; }
}
