using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Domain.Models
{
    public record CalculatedCustomerProduct(CustomerRegistrationNumber CustomerRegistrationNumber, Product ProductPrice, Product ProductAmount, Product FinalTotal)
    {
        public int ProductId { get; set; }
        public bool IsUpdated { get; set; } 
    }
}
