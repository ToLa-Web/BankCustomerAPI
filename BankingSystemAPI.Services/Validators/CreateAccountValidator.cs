using BankingSystemAPI.Core.DTOs.Request.AccountRequest;
using BankingSystemAPI.Core.DTOs.Response;
using BankingSystemAPI.Core.Enums;

namespace BankingSystemAPI.Services.Validators;

public static class CreateAccountValidator
{
    private static readonly HashSet<string> SupportedCurrencies =
        new() { "USD", "EUR", "KHR", "CNY" };

    public static ResultDto Validate(CreateAccountDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Currency))
            return new ResultDto {Success = false, Message = "Currency is required"};

        var currency = dto.Currency.Trim().ToUpperInvariant();

        if (currency.Length != 3)
            return new ResultDto { Success = false, Message = "Currency must be a 3-letter ISO code"};

        if (!SupportedCurrencies.Contains(currency))
            return new ResultDto { Success = false, Message = $"Unsupported currency: {currency}" };

        if (dto.AccountType == AccountType.FixedDeposit &&
            (!dto.FixedTermMonths.HasValue || dto.FixedTermMonths <= 0))
            return new ResultDto { Success = false, Message = "FixedTermMonths is required for fixed deposit accounts"};

        return ResultDto.Ok();
    }
}