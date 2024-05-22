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
        /// ���o�Ȥ�M�� (�ϥ� ActionResult<T>)
        /// </summary>
        /// <remarks>
        /// ### �y�{
        /// 1. ����h������ HTTP GET �ШD��A�եΪA�ȼh��k `GetAllCustomersAsync`�C
        /// 2. �A�ȼh��k����~���޿�A�ýե��x�s�w�h��k������ƾڡC
        /// 3. �x�s�w�h�ϥ� DbContext �d�߼ƾڮw�A�ê�^�Ȥ�ƾڵ��A�ȼh�C
        /// 4. �A�ȼh������ƾګ�A�N���^������h�C
        /// 5. ����h�N�ƾګʸ˦b Ok ���G���A�ê�^���Ȥ�ݡC
        /// </remarks>
        /// <returns>�Ҧ��Ȥ᪺�M��</returns>
        [HttpGet("CustomersOne")]
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
        /// 1. ����h������ HTTP GET �ШD��A�եΪA�ȼh��k `GetAllCustomersAsyncTwo`�C
        /// 2. �A�ȼh��k����~���޿�A�ýե��x�s�w�h��k������ƾڡC
        /// 3. �x�s�w�h�ϥ� DbContext �d�߼ƾڮw�A�ê�^�Ȥ�ƾڵ��A�ȼh�C
        /// 4. �A�ȼh������ƾګ�A�N���^������h�C
        /// 5. ����h�N�ƾګʸ˦b Ok ���G���A�ê�^���Ȥ�ݡC
        /// </remarks>
        /// <returns>�]�t�Ҧ��Ȥ᪺�M�檺 CustomerListRsModel</returns>
        [HttpGet("CustomersTwo")]
        public async Task<BaseResponseModel<CustomerListRsModel>> GetCustomersTwo()
        {
            var customers = await _customerService.GetAllCustomersAsyncTwo();
            return customers;
        }

        /// <summary>
        /// �ھګȤ�ID���o�S�w�Ȥ���
        /// </summary>
        /// <remarks>
        /// ### �y�{
        /// 1. ����h������ HTTP GET �ШD��A�եΪA�ȼh��k `GetCustomerByIdAsync`�C
        /// 2. �A�ȼh��k����~���޿�A�ýե��x�s�w�h��k������ƾڡC
        /// 3. �x�s�w�h�ϥ� DbContext �d�߼ƾڮw�A�ê�^�Ȥ�ƾڵ��A�ȼh�C
        /// 4. �A�ȼh������ƾګ�A�N���^������h�C
        /// 5. ����h�N�ƾګʸ˦b Ok ���G���A�ê�^���Ȥ�ݡC
        /// </remarks>
        /// <param name="id">�Ȥ�ID</param>
        /// <returns>�S�w�Ȥ᪺���</returns>
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
        /// �s�W�Ȥ�
        /// </summary>
        /// <remarks>
        /// ### �y�{
        /// 1. ����h������ HTTP POST �ШD��A�եΪA�ȼh��k `AddCustomerAsync`�C
        /// 2. �A�ȼh��k����~���޿�A�ýե��x�s�w�h��k�ӫO�s�ƾڡC
        /// 3. �x�s�w�h�ϥ� DbContext �N�Ȥ�ƾګO�s��ƾڮw���C
        /// 4. �A�ȼh������O�s���\�����G��A�N���^������h�C
        /// 5. ����h�N�ƾګʸ˦b CreatedAtAction ���G���A�ê�^���Ȥ�ݡC
        /// </remarks>
        /// <param name="customer">�Ȥ���</param>
        /// <returns>�s�W���Ȥ���</returns>
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            await _customerService.AddCustomerAsync(customer);
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerID }, customer);
        }

        /// <summary>
        /// ��s�Ȥ���
        /// </summary>
        /// <remarks>
        /// ### �y�{
        /// 1. ����h������ HTTP PUT �ШD��A�եΪA�ȼh��k `UpdateCustomerAsync`�C
        /// 2. �A�ȼh��k����~���޿�A�ýե��x�s�w�h��k�ӧ�s�ƾڡC
        /// 3. �x�s�w�h�ϥ� DbContext ��s�ƾڮw�����Ȥ��ơC
        /// 4. �A�ȼh�������s���\�����G��A�N���^������h�C
        /// 5. ����h��^ NoContent ���G���Ȥ�ݡC
        /// </remarks>
        /// <param name="id">�Ȥ�ID</param>
        /// <param name="customer">�Ȥ���</param>
        /// <returns>��s���G</returns>
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
        /// �R���Ȥ�
        /// </summary>
        /// <remarks>
        /// ### �y�{
        /// 1. ����h������ HTTP DELETE �ШD��A�եΪA�ȼh��k `DeleteCustomerAsync`�C
        /// 2. �A�ȼh��k����~���޿�A�ýե��x�s�w�h��k�ӵ��O�R���ƾڡC
        /// 3. �x�s�w�h�ϥ� DbContext ���O�R���ƾڮw�����Ȥ��ơC
        /// 4. �A�ȼh��������O�R�����\�����G��A�N���^������h�C
        /// 5. ����h��^ NoContent ���G���Ȥ�ݡC
        /// </remarks>
        /// <param name="id">�Ȥ�ID</param>
        /// <returns>�R�����G</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(string id)
        {
            await _customerService.DeleteCustomerAsync(id);
            return NoContent();
        }
    }
}