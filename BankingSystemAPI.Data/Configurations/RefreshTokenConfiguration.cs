using BankingSystemAPI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingSystemAPI.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(r => r.RefreshTokenId);
        
        builder.Property(r => r.Token)
            .IsRequired()
            .HasMaxLength(256);
        builder.Property(r => r.ExpiresAt)
            .IsRequired();
        builder.Property(r => r.RevokedAt)
            .IsRequired(false);
        
        builder.HasIndex(r => r.Token)
            .IsUnique();
        
        builder.HasOne(r => r.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        
    }
}