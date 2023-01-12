using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Exemple.Domain.Models;

namespace Example.Api.Models
{
    public class InputProduct
    {
        [Required]
        [RegularExpression(CustomerRegistrationNumber.Pattern)]
        public string RegistrationNumber { get; set; }

        [Required]
        [Range(1, 100)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, 1000)]
        public decimal Amount { get; set; }
    }
}
