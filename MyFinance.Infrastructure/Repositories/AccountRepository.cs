using MyFinance.Common.Models;
using MyFinance.Core.Interfaces;
using MyFinance.Core.Models;
using MyFinance.Infrastructure.Data;

namespace MyFinance.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly FinanceDbContext _context;

        public AccountRepository(FinanceDbContext context)
        {
            _context = context;
        }

        public Account Add(Account account)
        {
            _context.Accounts.Add(account);

            return account;
        }

        public IEnumerable<Account> GetAll()
        {
            return _context.Accounts.ToList();
        }

        public bool Delete(int id)
        {
            var account = _context.Accounts.Find(id);
            if (account == null)
            {
                return false;
            }

            _context.Accounts.Remove(account);
            _context.SaveChanges();
            return true;
        }
    }
}
