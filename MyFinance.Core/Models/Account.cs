using System.ComponentModel.DataAnnotations;
using MyFinance.Common.Models;

namespace MyFinance.Core.Models
{
    public class Account : Entity
    {
        [Required]
        public string Name { get; private set; }

        public string Identifier { get; private set; }

        [Required]
        public Currency Currency { get; private set; }

        [Required]
        public decimal InitialBalance { get; set; }

        public List<Transaction> Transactions { get; set; } = new List<Transaction>();

        public Account(string name, Currency currency, decimal initialBalance, string identifier = "")
        {
            Name = name;
            Currency = currency;
            InitialBalance = initialBalance;
            Identifier = identifier;
        }

        public Account()
        {
            // Default constructor for EF Core
        }
    }
}