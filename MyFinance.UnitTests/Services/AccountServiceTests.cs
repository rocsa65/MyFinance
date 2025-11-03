using System.Collections.Generic;
using System.Linq;
using Moq;
using Xunit;
using MyFinance.Core.Models;
using MyFinance.Core.Interfaces;
using MyFinance.Core.Services;
using MyFinance.Common.Models;

namespace MyFinance.UnitTests.Services
{
    public class AccountServiceTests
    {
        [Fact]
        public void GetAll_ReturnsEmpty_WhenNoAccounts()
        {
            // Arrange
            var mockRepo = new Mock<IAccountRepository>();
            mockRepo.Setup(r => r.GetAll()).Returns(new List<Account>());
            var service = new AccountService(mockRepo.Object);

            // Act
            var result = service.GetAll();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetAll_ReturnsAccountDtos_WithCorrectBalance()
        {
            // Arrange
            var account = new Account("Test", Currency.USD, 100, "id1")
            {
                Transactions = new List<Transaction>
                {
                    new Transaction { Amount = 50, Type = TransactionType.Credit },
                    new Transaction { Amount = 20, Type = TransactionType.Debit }
                }
            };
            var mockRepo = new Mock<IAccountRepository>();
            mockRepo.Setup(r => r.GetAll()).Returns(new List<Account> { account });
            var service = new AccountService(mockRepo.Object);

            // Act
            var result = service.GetAll().ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal("Test", result[0].Name);
            Assert.Equal(Currency.USD, result[0].Currency);
            Assert.Equal(130, result[0].Balance); // 100 + 50 - 20
        }

        [Fact]
        public void Create_CreatesAndReturnsAccountDto()
        {
            // Arrange
            var mockRepo = new Mock<IAccountRepository>();
            mockRepo.Setup(r => r.Add(It.IsAny<Account>()))
                .Returns<Account>(a =>
                {
                    a.GetType().GetProperty("Id")?.SetValue(a, 1);
                    return a;
                });

            var service = new AccountService(mockRepo.Object);

            // Act
            var result = service.Create("Test", "id1", Currency.EUR, 200);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test", result.Name);
            Assert.Equal("id1", result.Identifier);
            Assert.Equal(Currency.EUR, result.Currency);
            Assert.Equal(200, result.Balance);
            Assert.Equal(1, result.Id);
        }
    }
}
