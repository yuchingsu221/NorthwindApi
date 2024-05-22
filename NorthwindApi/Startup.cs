using Domain.Models.Config;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using NorthwindApi.Domain.Interfaces;
using NorthwindApi.Infrastructure.Context;
using NorthwindApi.Infrastructure.Repositories;
using NorthwindApi.Services;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using System.Net.Security;
using Newtonsoft.Json.Serialization;
using NorthwindApi.Filters.Swagger;
using NorthwindApi.Filters;
using NorthwindApi;
using NorthwindApi.Models.Response;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }
    private readonly AppSetting Setting = new AppSetting();

    public void ConfigureServices(IServiceCollection services)
    {
        SetAppsettingsInfo(services);
        SetJSONFormatInfo(services);
        SetSwaggerInfo(services);
        SetNpgsqlConnectionInfo(services);
        SetCorsPolicyInfo(services);
        SetPluginDIInfo(services);
        SetFilterInfo(services);
        SetAttributeInfo(services);

        ServicePointManager.DefaultConnectionLimit = short.MaxValue;
        ServicePointManager.ServerCertificateValidationCallback +=
          (sender, cert, chain, sslPolicyErrors) =>
          {
              if (sslPolicyErrors == SslPolicyErrors.None)
              {
                  return true;
              }
              var request = sender as HttpWebRequest;
              if (request != null)
              {
                  var result = request.RequestUri.Host == Setting.HttpWebRequestHost;

                  return result;
              }
              return false;
          };

        // If using Kestrel:
        services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });
        // If using IIS:
        services.Configure<IISServerOptions>(options => { options.AllowSynchronousIO = true; });
    }

    private void SetAppsettingsInfo(IServiceCollection services)
    {
        Configuration.Bind(Setting);
        services.AddSingleton(Setting);
    }

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

    private void SetNpgsqlConnectionInfo(IServiceCollection services)
    {
        //services.AddTransient<IDbConnection>(db =>
        //    new SqlServerConnection(Setting.RelationDB.DIR_ConnectionString));
    }    

    private void SetPluginDIInfo(IServiceCollection services)
    {
        // Db context
        services.AddDbContext<NorthwindContext>(options =>
        options.UseSqlServer(Setting.RelationDB.NorthwindApi_ConnectionString));

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<CustomerService>();
    }

    private void SetFilterInfo(IServiceCollection services)
    {
        services.AddMvcCore(config =>
        {
            config.Filters.Add(new ExceptionFilter(Setting));
            config.Filters.Add(new ResourceFilter(Setting));
            config.Filters.Add(new ResultFilter(Setting));
        });
    }

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
                    x => x.Value
                        .Errors
                        .Select(e => e.ErrorMessage)
                        .ToList());

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

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Northwind API v1");
                c.RoutePrefix = string.Empty; // To serve the Swagger UI at the app's root (http://localhost:<port>/)
            });
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
