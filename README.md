# README: (.net 6 API 程式架構設計) Northwind API 專案啟動與請求處理流程

## 目錄
1. [專案啟動流程](#專案啟動流程)
2. [API 請求處理流程](#api-請求處理流程)
3. [異常處理流程](#異常處理流程)
4. [專案結構](#專案結構)

## 專案啟動流程

專案啟動流程從 `Program.cs` 開始，負責配置和啟動 ASP.NET Core 應用程式。以下是啟動流程的詳細步驟：

1. **`Program.cs`**
    - `Main` 方法是應用程式的入口點。
    - 記錄啟動資訊（版本號等）。
    - 呼叫 `CreateHostBuilder` 方法來配置和建構主機。
    - 呼叫 `Build().Run()` 方法啟動應用程式。

```csharp
public static void Main(string[] args)
{
    var tag = "Program.Main";
    var version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version;

    LogUtility.LogInfo($"=========================", tag);
    LogUtility.LogInfo($"NorthWind API 啟動!", tag);
    LogUtility.LogInfo($"=========================", tag);
    LogUtility.LogInfo($"Ver: {version}");

    CreateHostBuilder(args).Build().Run();
}

public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostContext, config) =>
        {
            var env = hostContext.HostingEnvironment;
            LogUtility.LogInfo($"env:{env.EnvironmentName}");

            var jsonPath = $"{Directory.GetCurrentDirectory()}/appsettings.json";
            var jsonEnvPath = $"{Directory.GetCurrentDirectory()}/appsettings.{env.EnvironmentName}.json";

            config.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(path: $"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Warning);
                })
                .UseNLog();
        });
```		
		
2. Startup.cs
ConfigureServices 方法
ConfigureServices 方法負責配置應用程式的服務，並將其添加到依賴注入容器中。

```csharp
public void ConfigureServices(IServiceCollection services)
{
    SetAppsettingsInfo(services);
    SetJSONFormatInfo(services);
    SetSwaggerInfo(services);
    SetCorsPolicyInfo(services);
    SetPluginDIInfo(services);
    SetFilterInfo(services);
    SetAttributeInfo(services);
}
```
SetAppsettingsInfo

將應用程式設定（AppSetting）綁定到服務容器中，以便在應用程式的其他部分使用。

```csharp
private void SetAppsettingsInfo(IServiceCollection services)
{
    Configuration.Bind(Setting);
    services.AddSingleton(Setting);
}
```
SetJSONFormatInfo

配置全局 JSON 序列化設定，確保應用程式以統一的格式處理 JSON 資料。

```csharp
private void SetJSONFormatInfo(IServiceCollection services)
{
    services.AddControllers();

    JsonConvert.DefaultSettings = () => new JsonSerializerSettings
    {
        Formatting = Formatting.None,
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        DateFormatString = "yyyy-MM-dd HH:mm:ss",
        NullValueHandling = NullValueHandling.Include,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    };
}
```
SetSwaggerInfo

配置 Swagger，用於生成 API 文檔，方便開發和測試。

```csharp
private void SetSwaggerInfo(IServiceCollection services)
{
    if (Setting.EnableSwagger)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new() { Title = "Northwind API", Version = "v1" });
        });
    }
}
```
SetCorsPolicyInfo

配置 CORS 策略，允許跨來源請求，提升應用程式的兼容性和靈活性。

```csharp
private void SetCorsPolicyInfo(IServiceCollection services)
{
    services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy", policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed(x => true);
        });
    });
}
```
SetPluginDIInfo

配置依賴注入，將需要的服務和存儲庫添加到容器中。

```csharp
private void SetPluginDIInfo(IServiceCollection services)
{
    // Db context
    services.AddDbContext<NorthwindContext>(options =>
        options.UseSqlServer(Setting.RelationDB.NorthwindApi_ConnectionString));

    services.AddScoped<ICustomerRepository, CustomerRepository>();
    services.AddScoped<CustomerService>();
}
```
SetFilterInfo

配置全局過濾器，用於統一處理請求和響應的邏輯，包括異常處理、資源過濾等。

```csharp
private void SetFilterInfo(IServiceCollection services)
{
    services.AddMvcCore(config =>
    {
        config.Filters.Add(new ExceptionFilter(Setting));
        config.Filters.Add(new ResourceFilter(Setting));
        config.Filters.Add(new ResultFilter(Setting));
    });
}
```
SetAttributeInfo

配置 API 行為，包括模型驗證錯誤的處理。

```csharp
private void SetAttributeInfo(IServiceCollection services)
{
    services.Configure<ApiBehaviorOptions>(options =>
    {
        options.SuppressInferBindingSourcesForParameters = true;
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                                .Where(p => p.Value.Errors.Count > 0)
                                .ToList();
            var keys = string.Join(Environment.NewLine, errors.Select(x => x.Key));

            var errMsgs = errors.ToDictionary(
                x => x.Key,
                x => x.Value.Errors.Select(e => e.ErrorMessage).ToList()
            );

            var errorMsg = string.Empty;
            var errorDefine = ErrorDefine.GetErrorDefine(ErrorCodeEnum.PARAMETER_ERR_CODE);
            if (errMsgs != null && errMsgs.Any())
            {
                foreach (var errMsg in errMsgs)
                {
                    errorMsg += string.Join(", ", errMsg.Value) + ", ";
                }

                errorMsg = errorMsg.Substring(0, errorMsg.Length - 2);
            }
            else
            {
                errorMsg = errorDefine.ErrorMsg;
            }

            var errResponse = new BaseResponseModel<string>
            {
                RtnCode = ((int)errorDefine.ErrorCode).ToString(),
                RtnMsg = errorMsg
            };

            var response = new BadRequestObjectResult(errResponse)
            {
                StatusCode = 200
            };

            return response;
        };
    });
}
```
Configure 方法
Configure 方法負責配置 HTTP 請求。

```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Northwind API v1");
            c.RoutePrefix = string.Empty;
        });
    }

    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseAuthorization();
    app.UseCors("CorsPolicy");
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}
```
API 請求處理流程
當客戶端發出 API 請求時，ASP.NET Core 會按以下順序處理請求：

請求進入管道

通過中介軟體，如 UseRouting、UseAuthorization 等。
路由到控制器

app.UseHttpsRedirection():
這個中介軟體將 HTTP 請求重定向到 HTTPS，確保所有請求都使用安全的 HTTPS 協議。

app.UseRouting():
這個中介軟體負責路由請求，將請求路由到相應的控制器和動作方法。

app.UseAuthorization():
這個中介軟體用於執行授權檢查，確保只有授權的用戶才能訪問受保護的資源。

app.UseCors("CorsPolicy"):
這個中介軟體配置跨來源資源共享 (CORS) 政策，允許或限制來自不同來源的請求。

app.UseEndpoints(endpoints => { endpoints.MapControllers(); }):
這個中介軟體將控制器的路由端點映射到具體的動作方法，處理最終的請求和響應。

根據請求 URL 路由到相應的控制器和操作方法。
操作方法處理請求

控制器操作方法執行業務邏輯。
呼叫服務層來處理具體的業務操作。
返回響應

操作方法執行完畢後，將結果返回給客戶端。
示例：CustomersController
```csharp
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly CustomerService _customerService;

    public CustomersController(CustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
    {
        var customers = await _customerService.GetAllCustomersAsync();
        return Ok(customers);
    }
}
```

異常處理流程
當 API 請求過程中發生異常時，異常處理流程如下：

拋出異常

業務邏輯或數據訪問層發生異常時，拋出異常。
自定義異常處理器捕獲異常

ExceptionFilter 捕獲未處理的異常。
根據異常類型和自定義邏輯，構建統一的錯誤響應。
示例：ExceptionFilter
```csharp
public class ExceptionFilter : IExceptionFilter
{
    private readonly AppSetting _setting;

    public ExceptionFilter(AppSetting setting)
    {
        _setting = setting;
    }

    public void OnException(ExceptionContext context)
    {
        var ex = context.Exception;
        var errorDefine = ex is CustomExceptionHandler customEx
            ? ErrorDefine.GetErrorDefine((ErrorCodeEnum)int.Parse(customEx.Message.Split("||")[0]))
            : ErrorDefine.GetErrorDefine(ErrorCodeEnum.EXCUTE_ERR_CODE);

        var result = new BaseResponseModel<string>
        {
            RtnCode = ((int)errorDefine.ErrorCode).ToString(),
            RtnMsg = errorDefine.ErrorMsg
        };

        context.Result = new ContentResult
        {
            Content = JsonConvert.SerializeObject(result),
            ContentType = "application/json",
            StatusCode = 200
        };

        context.ExceptionHandled = true;
    }
}
```
遇到 Guard.Throw 時的流程
Guard.Throw 用於在代碼中拋出自定義異常。流程如下：

調用 Guard.Throw

當業務邏輯檢測到錯誤條件時調用。
拋出 CustomExceptionHandler 異常

生成包含錯誤碼和自定義消息的異常。
異常過濾器處理異常

ExceptionFilter 捕獲 CustomExceptionHandler 異常。
根據異常中的錯誤碼構建統一的錯誤響應。
示例：Guard.Throw
```csharp
public class Guarder
{
    public static void Throw(ErrorCodeEnum errorCode, string customErrorMsg = "", Exception ex = null)
    {
        if (ex == null)
            throw new CustomExceptionHandler(errorCode, customErrorMsg);
        else
            throw ex;
    }
}
```
專案結構
```csharp
NorthwindApi
├── Controllers
│   └── CustomersController.cs
├── Domain
│   ├── Entities
│   │   └── Customer.cs
│   └── Interfaces
│       └── ICustomerRepository.cs
├── Filters
│   ├── ExceptionFilter.cs
│   ├── ResourceFilter.cs
│   └── ResultFilter.cs
├── Infrastructure
│   ├── Context
│   │   └── NorthwindContext.cs
│   └── Repositories
│       └── CustomerRepository.cs
├── Models
│   └── Response
│       ├── AspNetResultModel.cs
│       └── BaseResponseModel.cs
├── Services
│   ├── ApiBaseService.cs
│   └── CustomerService.cs
├── Util
│   └── LogUtility.cs
├── Program.cs
├── Startup.cs
└── appsettings.json
```
