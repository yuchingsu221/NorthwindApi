using System;
using System.Runtime.Serialization;

namespace NorthwindApi
{
    [Serializable]
    public class CustomExceptionHandler : Exception, ISerializable
    {
        public CustomExceptionHandler(ErrorCodeEnum errorCode, string customErrorMsg) : base(((int)errorCode).ToString() + "||" + customErrorMsg) { }
    }
}
