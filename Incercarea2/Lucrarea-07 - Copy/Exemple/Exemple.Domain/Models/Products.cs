using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Domain.Models
{
    [AsChoice]
    public static partial class Products
    {
        public interface IProducts { }

        public record UnvalidatedProducts : IProducts
        {
            public UnvalidatedProducts(IReadOnlyCollection<UnvalidatedCustomerProduct> productList)
            {
                ProductList = productList;
            }

            public IReadOnlyCollection<UnvalidatedCustomerProduct> ProductList { get; }
        }

        public record InvalidProducts : IProducts
        {
            internal InvalidProducts(IReadOnlyCollection<UnvalidatedCustomerProduct> productList, string reason)
            {
                ProductList = productList;
                Reason = reason;
            }

            public IReadOnlyCollection<UnvalidatedCustomerProduct> ProductList { get; }
            public string Reason { get; }
        }

        public record FailedProducts : IProducts
        {
            internal FailedProducts(IReadOnlyCollection<UnvalidatedCustomerProduct> productList, Exception exception)
            {
                ProductList = productList;
                Exception = exception;
            }

            public IReadOnlyCollection<UnvalidatedCustomerProduct> ProductList { get; }
            public Exception Exception { get; }
        }

        public record ValidatedProducts : IProducts
        {
            internal ValidatedProducts(IReadOnlyCollection<ValidatedCustomerProduct> productList)
            {
                ProductList = productList;
            }

            public IReadOnlyCollection<ValidatedCustomerProduct> ProductList { get; }
        }

        public record CalculatedProducts : IProducts
        {
            internal CalculatedProducts(IReadOnlyCollection<CalculatedCustomerProduct> productList)
            {
                ProductList = productList;
            }

            public IReadOnlyCollection<CalculatedCustomerProduct> ProductList { get; }
        }

        public record PublishedProducts : IProducts
        {
            internal PublishedProducts(IReadOnlyCollection<CalculatedCustomerProduct> productList, string csv, DateTime publishedDate)
            {
                ProductList = productList;
                PublishedDate = publishedDate;
                Csv = csv;
            }

            public IReadOnlyCollection<CalculatedCustomerProduct> ProductList { get; }
            public DateTime PublishedDate { get; }
            public string Csv { get; }
        }
    }
}
