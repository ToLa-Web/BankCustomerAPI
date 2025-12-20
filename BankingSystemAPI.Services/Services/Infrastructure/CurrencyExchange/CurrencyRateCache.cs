namespace BankingSystemAPI.Services.Services.Infrastructure.CurrencyExchange;

internal class CurrencyRateCache
{
    public DateTime LastUpdated { get; set; }
    public Dictionary<string, decimal> Rates { get; set; } = new();
}