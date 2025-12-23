using BankingSystemAPI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingSystemAPI.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");
        builder.HasKey(t => t.TransactionId);
        builder.HasIndex(t => t.TransactionReference).IsUnique();
        builder.Property(t => t.TransactionReference).IsRequired().HasMaxLength(64);
        builder.Property(t => t.Amount).HasPrecision(18,2);
        builder.Property(t => t.BalanceBefore).HasPrecision(18,2);
        builder.Property(t => t.BalanceAfter).HasPrecision(18,2);
        builder.Property(t => t.Status).IsRequired().HasMaxLength(20);
        builder.Property(t => t.Description)
            .HasMaxLength(255);
        builder.Property(t => t.RecipientAccount)
            .HasMaxLength(50);
        builder.Property(t => t.RecipientAccountName)
            .HasMaxLength(100);
        builder.HasOne(t => t.Account)
            .WithMany(a => a.Transactions)
            .HasForeignKey(t => t.AccountId);
    }
}