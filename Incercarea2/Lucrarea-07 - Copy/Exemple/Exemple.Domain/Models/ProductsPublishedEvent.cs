using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Domain.Models
{
    [AsChoice]
    public static partial class ProductsPublishedEvent
    {

        public interface IProductsPublishedEvent { }
        public record ProductsPublishScucceededEvent : IProductsPublishedEvent
        {
            public IEnumerable<PublishedCustomerProduct> Product{ get; }
            public DateTime PublishedDate { get; }

            internal ProductsPublishScucceededEvent(IEnumerable<PublishedCustomerProduct> product, DateTime publishedDate)
            {
                Product = product;
                PublishedDate = publishedDate;
            }
        }

        public record ProductsPublishFaildEvent : IProductsPublishedEvent
        {
            public string Reason { get; }

            internal ProductsPublishFaildEvent(string reason)
            {
                Reason = reason;
            }
        }
    }
}
