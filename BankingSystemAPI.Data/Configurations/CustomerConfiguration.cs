using BankingSystemAPI.Core.Entities;
using BankingSystemAPI.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingSystemAPI.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        
        builder.HasKey(c => c.CustomerId);
        builder.Property(c => c.UserId).IsRequired();
        builder.Property(c => c.NationalId)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(c => c.PhoneNumber)
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(c => c.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(c => c.LastName).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Address).IsRequired().HasMaxLength(500);
        builder.Property(c => c.DateOfBirth).IsRequired().HasColumnType("date");
        builder.Property(c => c.Gender).IsRequired().HasMaxLength(10);
        builder.Property(c => c.CustomerNumber).IsRequired();
        builder.Property(c => c.VerificationStatus)
            .HasConversion<string>()
            .HasDefaultValue(CustomerVerificationStatus.None)
            .IsRequired();
        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasDefaultValue(CustomerStatus.Active)
            .IsRequired();
        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
        builder.Property(c => c.UpdatedAt)
            .IsRequired(false);
        builder.Property(c => c.DeletedAt)
            .IsRequired(false);
        // Indexes
        builder.HasIndex(c => c.UserId).IsUnique().HasDatabaseName("IX_Customers_UserId"); 
        builder.HasIndex(c => c.NationalId).IsUnique();
        builder.HasIndex(c => c.PhoneNumber).IsUnique();
        builder.HasIndex(c => c.Status);
        // 1:1
        builder.HasOne(c => c.User)
            .WithOne(u => u.Customer)
            .HasForeignKey<Customer>(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        // 1:N
        builder.HasMany(c => c.Accounts)
            .WithOne(a => a.Customer)
            .HasForeignKey(a => a.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
        // 1:N
        builder.HasMany(c => c.Beneficiaries)
            .WithOne(b => b.Customer)
            .HasForeignKey(b => b.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
        // Query Filter for soft delete
        builder.HasQueryFilter(c => c.DeletedAt == null);
    }
}