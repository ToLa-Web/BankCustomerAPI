using BankingSystemAPI.Core.DTOs.Response;

namespace BankingSystemAPI.Core.Interfaces.Infrastructure;

public interface ICurrencyExchangeService
{
    Task<Result<decimal>> ConvertAsync(
        decimal amount, 
        string fromCurrency, 
        string toCurrency);
}