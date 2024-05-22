using NorthwindApi.Domain.Entities;
using System.Runtime.Serialization;

namespace NorthwindApi.Models.Response
{
    [DataContract]
    public class CustomerListRsModel
    {
        /// <summary>
        /// 客戶清單
        /// </summary>
        [DataMember]
        public IEnumerable<Customer> Customers { get; set; }
    }
}
