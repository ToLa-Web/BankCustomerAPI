namespace BankingSystemAPI.Core.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
}