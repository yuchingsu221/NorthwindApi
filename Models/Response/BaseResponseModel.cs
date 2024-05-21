using System;
using System.Runtime.Serialization;

namespace NorthwindApi.Models.Response
{
    [DataContract]
    public class BaseResponseModel
    {
        [DataMember]
        public string RtnCode { get; set; }
        [DataMember]
        public string RtnMsg { get; set; }

        public BaseResponseModel()
        {
            RtnCode = "0000";
        }
    }
}
