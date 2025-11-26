using BankingSystemAPI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingSystemAPI.Data.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLog");

        builder.HasKey(a => a.AuditLogId);
        builder.Property(a => a.Action)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(a => a.IpAddress)
            .HasMaxLength(50);
        
        builder.Property(a => a.Device)
            .HasMaxLength(300);

        builder.Property(a => a.Timestamp)
            .HasDefaultValueSql("GETUTCDATE()");
        
        builder.HasOne(a => a.User)
            .WithMany(u => u.AuditLogs)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => a.Timestamp);
        builder.HasIndex(a => a.Action);
    }
}