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
            _context.SaveChanges();
            return account;
        }

        public IEnumerable<Account> GetAll()
        {
            return _context.Accounts.ToList();
        }
    }
}
