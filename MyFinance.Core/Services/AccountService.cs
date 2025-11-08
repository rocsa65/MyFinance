using MyFinance.Common.Models;
using MyFinance.Core.Interfaces;
using MyFinance.Core.Models;

namespace MyFinance.Core.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        public AccountService(IAccountRepository acountRepository)
        {
            _accountRepository = acountRepository;
        }

        public AccountDto Create(string name, string? identifier, Currency currency, decimal balance)
        {
            identifier ??= string.Empty;

            var account = new Account(name, currency, balance, identifier);

            var createdAccount = _accountRepository.Add(account);

            var accountDto = new AccountDto
            {
                Id = createdAccount.Id,
                Name = createdAccount.Name,
                Identifier = createdAccount.Identifier,
                Currency = createdAccount.Currency,
                Balance = createdAccount.InitialBalance
            };

            return accountDto;
        }

        public IEnumerable<AccountDto> GetAll()
        {
            var accounts = _accountRepository.GetAll();

            if (accounts == null || !accounts.Any())
            {
                yield break;
            }

            foreach (var account in accounts)
            {
                var debit = account.Transactions
                    .Where(t => t.Type == TransactionType.Debit)
                    .Sum(t => t.Amount);

                var credit = account.Transactions
                    .Where(t => t.Type == TransactionType.Credit)
                    .Sum(t => t.Amount);

                var balance = account.InitialBalance + credit - debit;

                yield return new AccountDto
                {
                    Name = account.Name,
                    Currency = account.Currency,
                    Balance = balance
                };
            }
        }

        public bool Delete(int id)
        {
            return _accountRepository.Delete(id);
        }
    }
}
