namespace BankingSystemAPI.Core.DTOs.Response.ExchangeRates;

public class ExchangeRatesDto
{
    public string Base { get; set; } = "USD";
    public DateTime LastUpdated { get; set; }
    public Dictionary<string, decimal> Rates { get; set; } = new();
}