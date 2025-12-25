using BankingSystemAPI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingSystemAPI.Data.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");
        builder.HasKey(a => a.AccountId);
        builder.Property(a => a.AccountNumber)
            .IsRequired()
            .HasMaxLength(20);
        
        builder.Property(a => a.AccountType)
            .HasConversion<string>()
            .IsRequired();
        
        builder.Property(a => a.Currency)
            .HasMaxLength(3)
            .IsRequired();
        
        builder.Property(a => a.Balance)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0);
        
        builder.Property(a => a.InterestRate)
            .HasColumnType("decimal(18,6)")
            .HasDefaultValue(0);

        builder.Property(a => a.MaturityDate)
            .IsRequired(false);

        builder.Property(a => a.IsActive)
            .HasDefaultValue(true);
        builder.Property(a => a.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();
        
        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(a => a.UpdatedAt).IsRequired(false);
        
        builder.HasIndex(a => a.AccountNumber).IsUnique();
        
        builder.HasOne(a => a.Customer)
            .WithMany(c => c.Accounts)
            .HasForeignKey(a => a.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.Transactions)
            .WithOne(a => a.Account)
            .HasForeignKey(a => a.AccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}