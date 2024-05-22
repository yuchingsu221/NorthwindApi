using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using NorthwindApi.Domain.Entities;
using NorthwindApi.Domain.Interfaces;
using NorthwindApi.Services;

namespace NorthwindApi.Tests
{
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly CustomerService _customerService;

        public CustomerServiceTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _customerService = new CustomerService(_customerRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAllCustomersAsync_ShouldReturnAllCustomers()
        {
            // Arrange
            var customers = new List<Customer>
            {
                new Customer { CustomerID = "ALFKI", CompanyName = "Alfreds Futterkiste" },
                new Customer { CustomerID = "ANATR", CompanyName = "Ana Trujillo Emparedados y helados" }
            };
            _customerRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(customers);

            // Act
            var result = (await _customerService.GetAllCustomersAsync()).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Alfreds Futterkiste", result[0].CompanyName);
            //Ana Trujillo Emparedados y helados
            Assert.Equal("Customer 2", result[1].CompanyName);
        }

        [Fact]
        public async Task GetCustomerByIdAsync_ShouldReturnCustomer()
        {
            // Arrange
            var customer = new Customer { CustomerID = "ALFKI", CompanyName = "Alfreds Futterkiste" };
            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync("ALFKI")).ReturnsAsync(customer);

            // Act
            var result = await _customerService.GetCustomerByIdAsync("ALFKI");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Alfreds Futterkiste", result.CompanyName);
        }

        [Fact]
        public async Task DeleteCustomerAsync_ShouldCallDeleteOnce()
        {
            // Arrange
            var customerId = "ALFKI";

            // Act
            await _customerService.DeleteCustomerAsync(customerId);

            // Assert
            _customerRepositoryMock.Verify(repo => repo.DeleteAsync(customerId), Times.Once);
        }
    }
}
