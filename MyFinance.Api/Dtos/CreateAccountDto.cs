using MyFinance.Common.Models;

namespace MyFinance.Api.Dtos
{
    public class CreateAccountDto
    {
        public string Name { get; set; }
        public string? Identifier { get; set; }
        public Currency Currency { get; set; }
        public decimal Balance { get; set; }
    }
}
