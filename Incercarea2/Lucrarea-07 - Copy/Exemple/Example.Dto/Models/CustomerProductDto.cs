using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Dto.Models
{
    public record CustomerProductDto
    {
        public string Name { get; init; }
        public string CustomerRegistrationNumber { get; init; }
        public decimal ProductPrice { get; init; }
        public decimal ProductAmount { get; init; }     
        public decimal FinalTotal { get; init; }
    }
}
