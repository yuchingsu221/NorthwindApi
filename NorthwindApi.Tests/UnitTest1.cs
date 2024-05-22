using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using NorthwindApi.Domain.Entities;
using NorthwindApi.Domain.Interfaces;
using NorthwindApi.Services;
using System.Linq;

namespace NorthwindApi.Tests
{
    [TestClass]
    public class CustomerServiceTests
    {
        private Mock<ICustomerRepository> _customerRepositoryMock;
        private CustomerService _customerService;

        [TestInitialize]
        public void Setup()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _customerService = new CustomerService(_customerRepositoryMock.Object);
        }

        [TestMethod]
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
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Alfreds Futterkiste", result[0].CompanyName);
            Assert.AreEqual("Ana Trujillo Emparedados y helados", result[1].CompanyName);
        }

        [TestMethod]
        public async Task GetCustomerByIdAsync_ShouldReturnCustomer()
        {
            // Arrange
            var customer = new Customer { CustomerID = "ALFKI", CompanyName = "Alfreds Futterkiste" };
            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync("ALFKI")).ReturnsAsync(customer);

            // Act
            var result = await _customerService.GetCustomerByIdAsync("ALFKI");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Alfreds Futterkiste", result.CompanyName);
        }

        [TestMethod]
        public async Task GetCustomerByIdAsync_ShouldReturnNull_WhenCustomerNotFound()
        {
            // Arrange
            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync("ALFKI")).ReturnsAsync((Customer)null);

            // Act
            var result = await _customerService.GetCustomerByIdAsync("ALFKI");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task AddCustomerAsync_ShouldCallAddOnce()
        {
            // Arrange
            var customer = new Customer { CustomerID = "ALFKI", CompanyName = "Alfreds Futterkiste" };

            // Act
            await _customerService.AddCustomerAsync(customer);

            // Assert
            _customerRepositoryMock.Verify(repo => repo.AddAsync(customer), Times.Once);
        }

        [TestMethod]
        public async Task UpdateCustomerAsync_ShouldCallUpdateOnce()
        {
            // Arrange
            var customer = new Customer { CustomerID = "ALFKI", CompanyName = "Alfreds Futterkiste" };

            // Act
            await _customerService.UpdateCustomerAsync(customer);

            // Assert
            _customerRepositoryMock.Verify(repo => repo.UpdateAsync(customer), Times.Once);
        }

        [TestMethod]
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
