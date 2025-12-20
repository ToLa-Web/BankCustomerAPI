using System.Text.Json;
using BankingSystemAPI.Core.DTOs.Response;
using BankingSystemAPI.Core.Interfaces.Infrastructure;
using BankingSystemAPI.Core.settings;
using Microsoft.Extensions.Options;

namespace BankingSystemAPI.Services.Services.Infrastructure.CurrencyExchange;

public class CurrencyExchangeService : ICurrencyExchangeService
{
    private readonly HttpClient _httpClient;
    private readonly CurrencyExchangeSettings _exchangeSettings;
    private static CurrencyRateCache _cache = new();

    public CurrencyExchangeService(HttpClient httpClient, IOptions<CurrencyExchangeSettings> options)
    {
        _httpClient = httpClient;
        _exchangeSettings = options.Value;
    }

    public async Task<Result<decimal>> ConvertAsync(decimal amount, string fromCurrency, string toCurrency)
    {
        if (fromCurrency == toCurrency)
            return Result<decimal>.SuccessResult(amount);

        var ensureResult = await EnsureRatesAsync();

        if (!ensureResult.Success)
            return Result<decimal>.Fail(ensureResult.Message!);

        if (!_cache.Rates.TryGetValue(fromCurrency, out var fromRate)) //var fromRate = _cache.Rates[fromCurrency];
            return Result<decimal>.Fail($"Unsupported currency: {fromCurrency}");

        if (!_cache.Rates.TryGetValue(toCurrency, out var toRate)) //var toRate = _cache.Rates[toCurrency];
            return Result<decimal>.Fail($"Unsupported currency: {toCurrency}");

        var usdAmount = amount / fromRate;
        var converted = usdAmount * toRate;

        return Result<decimal>.SuccessResult(converted);
    }

    private async Task<Result<bool>> EnsureRatesAsync()
    {
        if (_cache.Rates.Count != 0 && _cache.LastUpdated.Date == DateTime.UtcNow.Date)
            return Result<bool>.SuccessResult(false);

        try
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"{_exchangeSettings.BaseUrl}/latest?base=USD");

            request.Headers.Add("apikey", _exchangeSettings.ApiKey);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return Result<bool>.Fail("Failed to fetch currency rates");

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            var rates = doc.RootElement.GetProperty("rates");

            _cache.Rates = rates
                .EnumerateObject()
                .ToDictionary(
                    x => x.Name,
                    x => x.Value.GetDecimal());

            _cache.Rates["USD"] = 1m;
            _cache.LastUpdated = DateTime.UtcNow;

            return Result<bool>.SuccessResult(true);
        }
        catch
        {
            // Only catch infrastructure errors here
            return Result<bool>.Fail("Currency exchange service unavailable");
        }
    }
}