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
        /// 取得客戶清單 (使用ActionResult<T>)
        /// </summary>
        /// <remarks>
        /// ### 流程
        /// 1. 控制器層 接收到HTTP GET請求後，調用 服務層 方法。
        /// 2. 服務層 方法執行業務邏輯，並調用 儲存庫層 方法來獲取數據。
        /// 3. 儲存庫層 使用 DbContext 查詢數據庫，並返回客戶數據給 服務層。
        /// 4. 服務層 接收到數據後，將其返回給 控制器層。
        /// 5. 控制器層 將數據封裝在 Ok 結果中，並返回給客戶端。
        /// </remarks>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }

        /// <summary>
        /// 取得客戶清單 (使用指定型別)
        /// </summary>
        /// <remarks>
        /// ### 流程
        /// 1. 控制器層 接收到HTTP GET請求後，調用 服務層 方法。
        /// 2. 服務層 方法執行業務邏輯，並調用 儲存庫層 方法來獲取數據。
        /// 3. 儲存庫層 使用 DbContext 查詢數據庫，並返回客戶數據給 服務層。
        /// 4. 服務層 接收到數據後，將其返回給 控制器層。
        /// 5. 控制器層 將數據封裝在 Ok 結果中，並返回給客戶端。
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
