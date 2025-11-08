using MyFinance.Common.Models;
using MyFinance.Core.Models;

namespace MyFinance.Core.Services
{
    public interface IAccountService
    {
        IEnumerable<AccountDto> GetAll();

        AccountDto Create(string name, string? identifier, Currency currency, decimal balance);

        // Deletes account by id. Returns true if deletion succeeded (found and removed).
        bool Delete(int id);
    }
}
