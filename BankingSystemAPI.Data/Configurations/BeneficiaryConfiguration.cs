using BankingSystemAPI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingSystemAPI.Data.Configurations;

public class BeneficiaryConfiguration : IEntityTypeConfiguration<Beneficiary>
{
    public void Configure(EntityTypeBuilder<Beneficiary> builder)
    {
        builder.HasKey(b => b.BeneficiaryId);
        
        builder.Property(b => b.BeneficiaryName).IsRequired().HasMaxLength(100);
        builder.Property(b => b.AccountNumber).IsRequired().HasMaxLength(30);
        builder.Property(b => b.BankCode).IsRequired().HasMaxLength(20);
        builder.Property(b => b.BankName).IsRequired().HasMaxLength(150);
        
        // Prevent duplicate beneficiary per customer per bank
        builder.HasIndex(b => new { b.CustomerId, b.AccountNumber, b.BankCode })
            .IsUnique()
            .HasDatabaseName("IX_Beneficiaries_Customer_Account_Bank");
        
        builder.HasOne(b => b.Customer)
            .WithMany(c => c.Beneficiaries)
            .HasForeignKey(b => b.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}