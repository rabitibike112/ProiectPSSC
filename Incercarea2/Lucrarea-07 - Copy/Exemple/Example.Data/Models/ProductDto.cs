using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Data.Models
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public decimal? Price { get; set; }
        public decimal? Amount { get; set; }
        public decimal? TotalPrice { get; set; }
    }
}
