using BankingSystemAPI.Core.Entities;
using BankingSystemAPI.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingSystemAPI.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.UserId);
        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(u => u.PasswordSalt)
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(u => u.Role)
            .HasConversion<string>()
            .HasDefaultValue(UserRole.Customer)
            .IsRequired();
        builder.Property(u => u.IsEmailVerified)
            .HasDefaultValue(false)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(u => u.IsActive)
            .HasDefaultValue(true);
        
        //Unique Constraints
        builder.HasIndex(u => u.Email)
            .IsUnique();
        
        //One-to-One with Customer (optional - only for Customer role)
         builder.HasOne(u => u.Customer)
             .WithOne(c => c.User)
             .HasForeignKey<Customer>(c => c.UserId)
             .OnDelete(DeleteBehavior.Cascade);
    }
}