using MyFinance.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace MyFinance.Core.Models
{
    public class Transaction : Entity
    {
        [Required]
        public DateTime Date { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public TransactionType Type { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        public Transaction(DateTime date, decimal amount, string description = "")
        {
            Date = date;
            Amount = amount;
            Type = amount >= 0 ? TransactionType.Credit : TransactionType.Debit;
            Description = description;
        }

        public Transaction()
        {
            // Default constructor for EF Core
        }
    }
}