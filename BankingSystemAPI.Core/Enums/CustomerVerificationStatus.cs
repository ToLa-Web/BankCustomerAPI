namespace BankingSystemAPI.Core.Enums;

public enum CustomerVerificationStatus
{
    None,       // Not started
    Pending,    // Verification in progress
    Verified,   // KYC completed
    Rejected    // Verification failed
}