using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Exemple.Domain.Models.Products;

namespace Exemple.Domain.Models
{
    public record PublishProductCommand
    {
        public PublishProductCommand(IReadOnlyCollection<UnvalidatedCustomerProduct> inputProduct)
        {
            InputProduct = inputProduct;
        }

        public IReadOnlyCollection<UnvalidatedCustomerProduct> InputProduct { get; }
    }
}
