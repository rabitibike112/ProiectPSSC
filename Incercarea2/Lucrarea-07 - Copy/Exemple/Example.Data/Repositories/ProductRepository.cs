using Exemple.Domain.Models;
using Exemple.Domain.Repositories;
using LanguageExt;

using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using static LanguageExt.Prelude;
using Example.Data.Models;
using static Exemple.Domain.Models.Products;
using Example.Data;

namespace Priceple.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductContext dbContext;

        public ProductRepository(ProductContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public TryAsync<List<CalculatedCustomerProduct>> TryGetExistingProduct() => async () => (await (
                          from g in dbContext.Product
                          join s in dbContext.Customers on g.CustomerId equals s.CustomerId
                          select new { s.RegistrationNumber, g.ProductId, g.Price, g.Amount, g.TotalPrice })
                          .AsNoTracking()
                          .ToListAsync())
                          .Select(result => new CalculatedCustomerProduct(
                                                    CustomerRegistrationNumber: new(result.RegistrationNumber),
                                                    ProductPrice: new(result.Price ?? 0m),
                                                    ProductAmount: new(result.Amount ?? 0m),
                                                    FinalTotal: new(result.TotalPrice ?? 0m))
                          { 
                            ProductId = result.ProductId
                          })
                          .ToList();

        public TryAsync<Unit> TrySaveProduct(PublishedProducts product) => async () =>
        {
            var customers = (await dbContext.Customers.ToListAsync()).ToLookup(customer=>customer.RegistrationNumber);
            var newProduct = product.ProductList
                                    .Where(g => g.IsUpdated && g.ProductId == 0)
                                    .Select(g => new ProductDto()
                                    {
                                        CustomerId = customers[g.CustomerRegistrationNumber.Value].Single().CustomerId,
                                        Price = g.ProductPrice.Value,
                                        Amount = g.ProductAmount.Value,
                                        TotalPrice = g.FinalTotal.Value,
                                    });
            var updatedProduct = product.ProductList.Where(g => g.IsUpdated && g.ProductId > 0)
                                    .Select(g => new ProductDto()
                                    {
                                        ProductId = g.ProductId,
                                        CustomerId = customers[g.CustomerRegistrationNumber.Value].Single().CustomerId,
                                        Price = g.ProductPrice.Value,
                                        Amount = g.ProductAmount.Value,
                                        TotalPrice = g.FinalTotal.Value,
                                    });

            dbContext.AddRange(newProduct);
            foreach (var entity in updatedProduct)
            {
                dbContext.Entry(entity).State = EntityState.Modified;
            }

            await dbContext.SaveChangesAsync();

            return unit;
        };
    }
}
