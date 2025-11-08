using MyFinance.Common.Models;
using MyFinance.Core.Models;

namespace MyFinance.Core.Interfaces
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAll();

        Account Add(Account account);

        bool Delete(int id);
    }
}
