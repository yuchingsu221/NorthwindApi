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
        /// 取得客戶清單 (使用 ActionResult<T>)
        /// </summary>
        /// <remarks>
        /// ### 流程
        /// 1. 控制器層接收到 HTTP GET 請求後，調用服務層方法 `GetAllCustomersAsync`。
        /// 2. 服務層方法執行業務邏輯，並調用儲存庫層方法來獲取數據。
        /// 3. 儲存庫層使用 DbContext 查詢數據庫，並返回客戶數據給服務層。
        /// 4. 服務層接收到數據後，將其返回給控制器層。
        /// 5. 控制器層將數據封裝在 Ok 結果中，並返回給客戶端。
        /// </remarks>
        /// <returns>所有客戶的清單</returns>
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
        /// 1. 控制器層接收到 HTTP GET 請求後，調用服務層方法 `GetAllCustomersAsyncTwo`。
        /// 2. 服務層方法執行業務邏輯，並調用儲存庫層方法來獲取數據。
        /// 3. 儲存庫層使用 DbContext 查詢數據庫，並返回客戶數據給服務層。
        /// 4. 服務層接收到數據後，將其返回給控制器層。
        /// 5. 控制器層將數據封裝在 Ok 結果中，並返回給客戶端。
        /// </remarks>
        /// <returns>包含所有客戶的清單的 CustomerListRsModel</returns>
        [HttpGet("CustomersTwo")]
        public async Task<CustomerListRsModel> GetCustomersTwo()
        {
            var customers = await _customerService.GetAllCustomersAsyncTwo();
            return customers;
        }

        /// <summary>
        /// 根據客戶ID取得特定客戶資料
        /// </summary>
        /// <remarks>
        /// ### 流程
        /// 1. 控制器層接收到 HTTP GET 請求後，調用服務層方法 `GetCustomerByIdAsync`。
        /// 2. 服務層方法執行業務邏輯，並調用儲存庫層方法來獲取數據。
        /// 3. 儲存庫層使用 DbContext 查詢數據庫，並返回客戶數據給服務層。
        /// 4. 服務層接收到數據後，將其返回給控制器層。
        /// 5. 控制器層將數據封裝在 Ok 結果中，並返回給客戶端。
        /// </remarks>
        /// <param name="id">客戶ID</param>
        /// <returns>特定客戶的資料</returns>
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

        /// <summary>
        /// 新增客戶
        /// </summary>
        /// <remarks>
        /// ### 流程
        /// 1. 控制器層接收到 HTTP POST 請求後，調用服務層方法 `AddCustomerAsync`。
        /// 2. 服務層方法執行業務邏輯，並調用儲存庫層方法來保存數據。
        /// 3. 儲存庫層使用 DbContext 將客戶數據保存到數據庫中。
        /// 4. 服務層接收到保存成功的結果後，將其返回給控制器層。
        /// 5. 控制器層將數據封裝在 CreatedAtAction 結果中，並返回給客戶端。
        /// </remarks>
        /// <param name="customer">客戶資料</param>
        /// <returns>新增的客戶資料</returns>
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            await _customerService.AddCustomerAsync(customer);
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerID }, customer);
        }

        /// <summary>
        /// 更新客戶資料
        /// </summary>
        /// <remarks>
        /// ### 流程
        /// 1. 控制器層接收到 HTTP PUT 請求後，調用服務層方法 `UpdateCustomerAsync`。
        /// 2. 服務層方法執行業務邏輯，並調用儲存庫層方法來更新數據。
        /// 3. 儲存庫層使用 DbContext 更新數據庫中的客戶資料。
        /// 4. 服務層接收到更新成功的結果後，將其返回給控制器層。
        /// 5. 控制器層返回 NoContent 結果給客戶端。
        /// </remarks>
        /// <param name="id">客戶ID</param>
        /// <param name="customer">客戶資料</param>
        /// <returns>更新結果</returns>
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

        /// <summary>
        /// 刪除客戶
        /// </summary>
        /// <remarks>
        /// ### 流程
        /// 1. 控制器層接收到 HTTP DELETE 請求後，調用服務層方法 `DeleteCustomerAsync`。
        /// 2. 服務層方法執行業務邏輯，並調用儲存庫層方法來註記刪除數據。
        /// 3. 儲存庫層使用 DbContext 註記刪除數據庫中的客戶資料。
        /// 4. 服務層接收到註記刪除成功的結果後，將其返回給控制器層。
        /// 5. 控制器層返回 NoContent 結果給客戶端。
        /// </remarks>
        /// <param name="id">客戶ID</param>
        /// <returns>刪除結果</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(string id)
        {
            await _customerService.DeleteCustomerAsync(id);
            return NoContent();
        }
    }
}
