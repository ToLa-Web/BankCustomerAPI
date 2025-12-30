using BankingSystemAPI.Core.Entities;
using BankingSystemAPI.Core.Enums;
using BankingSystemAPI.Core.Helpers;
using BankingSystemAPI.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.Data.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(BankingSystemDbContext db)
    {
        // Ensure database exists
        await db.Database.MigrateAsync();

        // ===== USERS =====
        if (!await db.Users.AnyAsync())
        {
            // ===== Admin =====
            var (adminHash, adminSalt) = PasswordHasher.HashPassword("0987654321");
            
            db.Users.Add(new User
            {
                Email = "admin@bank.com",
                PasswordHash = adminHash,
                PasswordSalt = adminSalt,
                Role = UserRole.Administrator,
                IsEmailVerified = true
            });

            // ===== Staff =====
            var (staffHash, staffSalt) = PasswordHasher.HashPassword("1234567890");

            db.Users.Add(new User
            {
                Email = "staff@bank.com",
                PasswordHash = staffHash,
                PasswordSalt = staffSalt,
                Role = UserRole.Staff,
                IsEmailVerified = true
            });

            // ===== Customer =====
            var (customerHash, customerSalt) = PasswordHasher.HashPassword("1234567890");

            db.Users.AddRange(new User
            {
                Email = "customer1@bank.com",
                PasswordHash = customerHash,
                PasswordSalt = customerSalt,
                Role = UserRole.Customer,
                IsEmailVerified = true
            },
            new User
            {
                Email = "customer2@bank.com",
                PasswordHash = customerHash,
                PasswordSalt = customerSalt,
                IsEmailVerified = true
            },
            new User
            {
                Email = "customer3@bank.com",
                PasswordHash = customerHash,
                PasswordSalt = customerSalt,
                IsEmailVerified = true
            }
            );

            await db.SaveChangesAsync();
        }
        // ===== OPTIONAL FUTURE SEEDING =====
        // Customers
        // Accounts
        // Transactions
    }
}