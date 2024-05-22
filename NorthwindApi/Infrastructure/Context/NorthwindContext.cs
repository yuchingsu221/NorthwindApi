using Microsoft.EntityFrameworkCore;
using NorthwindApi.Domain.Entities;
using System.Collections.Generic;

namespace NorthwindApi.Infrastructure.Context
{
    public class NorthwindContext : DbContext
    {
        public NorthwindContext(DbContextOptions<NorthwindContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
    }
}
