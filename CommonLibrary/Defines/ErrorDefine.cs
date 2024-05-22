using System.Collections.Generic;
using System.Linq;

namespace NorthwindApi
{
    public class ErrorDefine
    {
        public ErrorCodeEnum ErrorCode { get; set; }
        public string ErrorMsg { get; set; }

        public static ErrorDefine GetErrorDefine(ErrorCodeEnum errorCode)
        {
            var errInfo = ErrorCodeAndMsgDefine.ERROR_MAPS.Where(x => x.ErrorCode == errorCode).FirstOrDefault();

            if (errInfo != null)
            {
                var errCode = errInfo.ErrorCode;
                var errMsg = errInfo.ErrorMsg;

                return new ErrorDefine
                {
                    ErrorCode = errCode,
                    ErrorMsg = errMsg
                };
            }

            return new ErrorDefine { ErrorCode = errorCode };
        }
    }

    public class ErrorCodeAndMsgDefine
    {
        public static readonly List<ErrorDefine> ERROR_MAPS = new List<ErrorDefine>
        {
            #region 總類
            new ErrorDefine()
            {
                ErrorCode = ErrorCodeEnum.SUCCESS_CODE,
                ErrorMsg = "成功"
            },
            new ErrorDefine()
            {
                ErrorCode = ErrorCodeEnum.PARAMETER_ERR_CODE,
                ErrorMsg = "參數錯誤，請檢查請求是否正確。"
            },
            new ErrorDefine()
            {
                ErrorCode = ErrorCodeEnum.EXCUTE_ERR_CODE,
                ErrorMsg = "系統忙線中，請稍後再試。"
            },
            #endregion      
        };
    }
}