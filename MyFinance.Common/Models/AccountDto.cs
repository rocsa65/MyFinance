namespace MyFinance.Common.Models
{
    public class AccountDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Identifier { get; set; }

        public Currency Currency { get; set; }

        public decimal Balance { get; set; }
    }
}
