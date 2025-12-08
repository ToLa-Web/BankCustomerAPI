using System.Text.RegularExpressions;

namespace BankingSystemAPI.Services.Validators;

public partial class CustomerValidator
{
    public static bool IsValidPhoneNumber(string phoneNumber)
    {
        return MyRegex().IsMatch(phoneNumber);
    }

    public static bool IsValidNationalId(string nationalId)
    {
        return MyRegex1().IsMatch(nationalId);
    }

    [GeneratedRegex(@"^0\d{7,9}$")]
    private static partial Regex MyRegex();
    [GeneratedRegex(@"^\d{9}$|^\d{12}$")]
    private static partial Regex MyRegex1();
}