using System.Collections.Generic;
using System.Threading.Tasks;
using NorthwindApi.Domain.Entities;
using NorthwindApi.Domain.Interfaces;
using NorthwindApi.Models.Response;

namespace NorthwindApi.Services
{
    public class CustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _customerRepository.GetAllAsync();
        }

        public async Task<CustomerListRsModel> GetAllCustomersAsyncTwo()
        {
            IEnumerable<Customer> customers = await _customerRepository.GetAllAsync();
            CustomerListRsModel customerListRsModel = new CustomerListRsModel();
            customerListRsModel.Customers = customers;
            return customerListRsModel;
        }

        public async Task<Customer> GetCustomerByIdAsync(string id)
        {
            return await _customerRepository.GetByIdAsync(id);
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            await _customerRepository.AddAsync(customer);
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            await _customerRepository.UpdateAsync(customer);
        }

        public async Task DeleteCustomerAsync(string id)
        {
            await _customerRepository.DeleteAsync(id);
        }
    }
}
