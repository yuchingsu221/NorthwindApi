using System.Collections.Generic;
using System.Threading.Tasks;
using NorthwindApi.Domain.Entities;

namespace NorthwindApi.Domain.Interfaces
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer> GetByIdAsync(string id);
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(string id);
    }
}
