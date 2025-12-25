using BankingSystemAPI.Core.DTOs.Response;
using BankingSystemAPI.Core.DTOs.Response.ExchangeRates;

namespace BankingSystemAPI.Core.Interfaces.Infrastructure;

public interface ICurrencyExchangeService
{
    Task<Result<decimal>> ConvertAsync(decimal amount, string fromCurrency, string toCurrency);
    Task<Result<ExchangeRatesDto>>  GetExchangeRatesAsync();
}