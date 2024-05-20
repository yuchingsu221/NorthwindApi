using Microsoft.EntityFrameworkCore;
using NorthwindApi.Domain.Interfaces;
using NorthwindApi.Infrastructure.Context;
using NorthwindApi.Infrastructure.Repositories;
using NorthwindApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<NorthwindContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("NorthwindDatabase")));
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
