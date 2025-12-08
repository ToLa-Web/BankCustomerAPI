namespace BankingSystemAPI.Core.Enums;

public enum CustomerStatus
{
    Active = 0,     // Normal account
    Suspended = 1,  // Temporarily blocked
    Closed = 2     // Permanently closed
}