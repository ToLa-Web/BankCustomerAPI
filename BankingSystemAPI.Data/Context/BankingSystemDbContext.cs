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
        modelBuilder.ApplyConfiguration(new AccountConfiguration());
        modelBuilder.ApplyConfiguration(new TransactionConfiguration());
    }
}