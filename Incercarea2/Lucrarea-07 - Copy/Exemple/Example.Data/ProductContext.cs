using Example.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Data
{
    public class ProductContext: DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options)
        {
        }

        public DbSet<ProductDto> Product { get; set; }

        public DbSet<CustomerDto> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerDto>().ToTable("Customer").HasKey(s => s.CustomerId);
            modelBuilder.Entity<ProductDto>().ToTable("Product").HasKey(s => s.ProductId);
        }
    }
}
