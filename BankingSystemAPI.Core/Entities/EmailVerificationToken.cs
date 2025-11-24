using BankingSystemAPI.Core.Enums;

namespace BankingSystemAPI.Core.Entities;

public class EmailVerificationToken
{
    public int TokenId { get; set; }
    public int UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public TokenType TokenType { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DateTime? UsedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public User? User { get; set; }  // navigation property
}