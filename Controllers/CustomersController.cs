using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NorthwindApi.Domain.Entities;
using NorthwindApi.Services;
using NorthwindApi.Models.Response;

namespace NorthwindApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly CustomerService _customerService;

        public CustomersController(CustomerService customerService)
        {
            _customerService = customerService;
        }


        /// <summary>
        /// ���o�Ȥ�M�� (�ϥ�ActionResult<T>)
        /// </summary>
        /// <remarks>
        /// ### �y�{
        /// 1. ����h ������HTTP GET�ШD��A�ե� �A�ȼh ��k�C
        /// 2. �A�ȼh ��k����~���޿�A�ýե� �x�s�w�h ��k������ƾڡC
        /// 3. �x�s�w�h �ϥ� DbContext �d�߼ƾڮw�A�ê�^�Ȥ�ƾڵ� �A�ȼh�C
        /// 4. �A�ȼh ������ƾګ�A�N���^�� ����h�C
        /// 5. ����h �N�ƾګʸ˦b Ok ���G���A�ê�^���Ȥ�ݡC
        /// </remarks>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }

        /// <summary>
        /// ���o�Ȥ�M�� (�ϥΫ��w���O)
        /// </summary>
        /// <remarks>
        /// ### �y�{
        /// 1. ����h ������HTTP GET�ШD��A�ե� �A�ȼh ��k�C
        /// 2. �A�ȼh ��k����~���޿�A�ýե� �x�s�w�h ��k������ƾڡC
        /// 3. �x�s�w�h �ϥ� DbContext �d�߼ƾڮw�A�ê�^�Ȥ�ƾڵ� �A�ȼh�C
        /// 4. �A�ȼh ������ƾګ�A�N���^�� ����h�C
        /// 5. ����h �N�ƾګʸ˦b Ok ���G���A�ê�^���Ȥ�ݡC
        /// </remarks>
        [HttpGet("CustomersTwo")]
        public async Task<CustomerListRsModel> GetCustomersTwo()
        {
            var customers = await _customerService.GetAllCustomersAsyncTwo();
            return customers;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(string id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            await _customerService.AddCustomerAsync(customer);
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerID }, customer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(string id, Customer customer)
        {
            if (id != customer.CustomerID)
            {
                return BadRequest();
            }

            await _customerService.UpdateCustomerAsync(customer);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(string id)
        {
            await _customerService.DeleteCustomerAsync(id);
            return NoContent();
        }
    }
}
