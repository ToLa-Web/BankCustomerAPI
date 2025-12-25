using BankingSystemAPI.Core.Interfaces.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemAPI.API.Controllers.ExchangeController;

[ApiController]
[Route("api/[controller]")]
public class ExchangeController : ControllerBase
{
    private readonly ICurrencyExchangeService _exchange;

    public ExchangeController(ICurrencyExchangeService exchange)
    {
        _exchange = exchange;
    }

    [HttpGet("rates")]
    public async Task<IActionResult> GetRates()
    {
        var result = await _exchange.GetExchangeRatesAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }
    
    [HttpGet("convert")]
    public async Task<IActionResult> Convert(decimal amount, string from, string to)
    {
        var result = await _exchange.ConvertAsync(amount, from, to);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}