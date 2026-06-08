using System.ComponentModel.DataAnnotations;

namespace ConsolidatedService.Models;

public class Consolidated
{
    [Key]
    public Guid Id { get; set; }
    public Guid MerchantId { get; set; }
    public DateTime Date { get; set; }
    public decimal Balance { get; set; }
}
