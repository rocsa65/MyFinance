using Microsoft.EntityFrameworkCore;
using MyFinance.Core.Models;

namespace MyFinance.Infrastructure.Data
{
    public class FinanceDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public FinanceDbContext(DbContextOptions<FinanceDbContext> options)
            : base(options)
        {
        }

        // Optionally, override OnModelCreating for custom configuration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Add custom configuration here if needed
        }
    }
}
