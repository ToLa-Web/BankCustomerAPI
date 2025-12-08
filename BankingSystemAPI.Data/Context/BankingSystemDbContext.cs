using BankingSystemAPI.Core.Entities;
using BankingSystemAPI.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.Data.Context;

public class BankingSystemDbContext : DbContext
{
    public BankingSystemDbContext(DbContextOptions<BankingSystemDbContext> options) : base(options)
    {}
    public DbSet<User> Users { get; set; }
    public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Account>  Accounts { get; set; }
    public DbSet<Beneficiary>  Beneficiaries { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //Apply Configuration
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new EmailVerificationTokenConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
        modelBuilder.ApplyConfiguration(new AuditLogConfiguration());


        // //User Configuration
        // modelBuilder.Entity<Customer>()
        //     .HasIndex(c => c.Email)
        //     .IsUnique();
        //
        // // Account Configuration
        // modelBuilder.Entity<Account>()
        //     .HasIndex(a => a.AccountNumber)
        //     .IsUnique();
        //
        // modelBuilder.Entity<Account>()
        //     .HasOne(a => a.Customer)
        //     .WithMany(c => c.Accounts)
        //     .HasForeignKey(a => a.CustomerId)
        //     .OnDelete(DeleteBehavior.Cascade);
        //
        // // Transaction Configuration
        // modelBuilder.Entity<Transaction>()
        //     .HasOne(t => t.Account)
        //     .WithMany(a => a.Transactions)
        //     .HasForeignKey(t => t.AccountId)
        //     .OnDelete(DeleteBehavior.Restrict);
        //
        // modelBuilder.Entity<Transaction>()
        //     .HasOne(t => t.ToAccount)
        //     .WithMany()
        //     .HasForeignKey(t => t.ToAccountId)
        //     .OnDelete(DeleteBehavior.Restrict);
    }
}