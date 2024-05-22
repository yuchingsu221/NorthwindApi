using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using NorthwindApi.Util;
using Domain.Models.Config;
using NorthwindApi;
using NorthwindApi.Models.Response;
using NorthwindApi.Services;

namespace NorthwindApi.Filters
{
    public class ResourceFilter : IResourceFilter
    {
        private static readonly object _CountLocker = new object();
        private static int INCounter = 0;
        private static AppSetting _Setting;

        public ResourceFilter(AppSetting setting)
        {
            _Setting = setting;
        }

        /// <summary>
        /// Response 介面實作
        /// </summary>
        /// <param name="context"></param>
        public void OnResourceExecuted(ResourceExecutedContext context)
        {

        }

        /// <summary>
        /// Request
        /// </summary>
        /// <param name="context"></param>
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            // TODO: 延長token時效
            var request = context.HttpContext.Request;
            var controller = context.RouteData.Values["Controller"].ToString();
            var action = context.RouteData.Values["Action"].ToString();
            var tag = $"{controller}.{action}";
            var requestContent = string.Empty;
            var requestNo = 0;

            // 記錄此次編號
            lock (_CountLocker)
            {
                INCounter++;
                requestNo = INCounter;
            }

            // ******** 開始判斷
            //if (!_Setting.EnableEncrypt)
            //{
                switch (request.Method.ToUpper())
                {
                    case "POST":
                        // 讀取Body 
                        request.EnableBuffering();
                        using (var stream = new StreamReader(stream: request.Body,
                                                     encoding: Encoding.UTF8,
                                                     detectEncodingFromByteOrderMarks: false,
                                                     bufferSize: 1024,
                                                     leaveOpen: true))
                        {
                            requestContent = stream.ReadToEndAsync().GetAwaiter().GetResult();
                        }
                        // 將資料放回 body
                        var bytes = Encoding.UTF8.GetBytes(requestContent);
                        request.Body = new MemoryStream(bytes);

                        break;

                    case "GET":
                        requestContent = request.QueryString.Value; // 直接將查詢字串放到
                        break;
                    default:
                        break;
                }

                try
                {
                    // 紀錄Log
                    if (!string.IsNullOrWhiteSpace(requestContent))
                    {
                        if (request.Method.ToUpper() == "POST")
                            requestContent = JObject.Parse(requestContent).ToString(Formatting.None);

                        if (!action.Equals("UploadDoc", StringComparison.OrdinalIgnoreCase))
                            LogUtility.LogInfo($"[IN={requestNo}]\r\nPA={requestContent}", tag);
                    }
                }
                catch (Exception ex) when (ex is Exception)
                {
                    LogUtility.LogError($"請求: {requestContent} 嘗試轉換 JSON 失敗", ex, tag);

                    PacketErrorResult(
                        context,
                        ErrorCodeEnum.EXCUTE_ERR_CODE,
                        requestNo,
                        "",
                        tag);

                    return;
                }
                context.HttpContext.Request.Headers.Add("OU", requestNo.ToString());

                return;
            //}
            LogUtility.LogInfo($"[Request Header ={JsonConvert.SerializeObject(request.Headers)}", tag);            
        }

        /// <summary>
        /// 直接組錯誤訊息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="errorCode"></param>
        /// <param name="requestNo"></param>
        /// <param name="aesKey"></param>
        /// <param name="tag"></param>
        private void PacketErrorResult(ResourceExecutingContext context, ErrorCodeEnum errorCode, int requestNo, string aesKey, string tag)
        {
            var result = ApiBaseService.BuildResponsePacket(new BaseResponseModel<string>(), errorCode);
            var response = JsonConvert.SerializeObject(result);

            context.Result = new ContentResult
            {
                Content = response,
                ContentType = "application/json",
                StatusCode = 200
            };

            LogUtility.LogInfo($"[OU={requestNo}]\r\nRS={response}", tag);
        }
    }
}
