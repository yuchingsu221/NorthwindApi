using Domain.Models.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using NorthwindApi.Models.Response;
using NorthwindApi.Util;

namespace NorthwindApi.Filters
{
    public class ResultFilter : IResultFilter
    {
        private static AppSetting _Setting;

        public ResultFilter(AppSetting setting)
        {
            _Setting = setting;
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            var request = context.HttpContext.Request;
            var requestNo = request.Headers["OU"].ToString();
            var response = context.HttpContext.Response;
            var controller = context.RouteData.Values["Controller"].ToString();
            var action = context.RouteData.Values["Action"].ToString();
            var aesKeyId = response.Headers["CRYPTO-ID"].ToString();
            var tag = $"{controller}.{action}";
            LogUtility.LogInfo($"[Request Header ={JsonConvert.SerializeObject(request.Headers)}", tag);

            // 解析從Action過來的物件
            var contextResult = context.Result;
            var aspNetResult = JsonConvert.DeserializeObject<AspNetResultModel>(JsonConvert.SerializeObject(contextResult));
            //var aesKeyValue = CryptoDefine.GetKeyValue(aesKeyId);
            var bussinessResult = JsonConvert.SerializeObject(aspNetResult.Value);

            LogUtility.LogInfo($"[OU={requestNo}]\r\nRS={bussinessResult}", tag);

            if (tag.Equals("Global.Key", StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new ContentResult
                {
                    Content = bussinessResult,
                    ContentType = "application/json",
                    StatusCode = 200
                };
                return;
            }

            //if (!_Setting.EnableEncrypt)
            //{
                context.Result = new ContentResult
                {
                    Content = bussinessResult,
                    ContentType = "application/json",
                    StatusCode = 200
                };

                return;
            //}

            //context.Result = new ContentResult
            //{
            //    Content = AESHelper.AES256GcmEncrypt(bussinessResult, aesKeyValue.AKey, CryptoDefine.A_IV),
            //    ContentType = "text/plain",
            //    StatusCode = 200
            //};

        }


        public void OnResultExecuted(ResultExecutedContext context)
        {

        }

    }
}
