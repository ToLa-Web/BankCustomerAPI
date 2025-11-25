namespace BankingSystemAPI.Core.settings;

public class RefreshTokenSetting
{
    public int ExpiryDays { get; set; } = 7;  // default 7 days
    public int TokenLength { get; set; } = 64; // optional: if generating random string
}