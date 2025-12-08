namespace BankingSystemAPI.Core.Enums;

public enum CustomerVerificationStatus
{
    None = 0,       // Not started
    Pending = 1,    // Verification in progress
    Verified = 2,   // KYC completed
    Rejected = 3   // Verification failed
}