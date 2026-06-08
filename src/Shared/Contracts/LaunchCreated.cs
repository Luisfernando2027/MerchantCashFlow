namespace MerchantCashFlow.Contracts;

public record LaunchCreated
{
    public Guid Id { get; init; }
    public Guid MerchantId { get; init; }
    public decimal Amount { get; init; }
    public DateTime OccurredAt { get; init; }
}
