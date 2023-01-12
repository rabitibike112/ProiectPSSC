using LanguageExt;
using System;
using static LanguageExt.Prelude;

namespace Exemple.Domain.Models
{
    public record Product
    {
        public decimal Value { get; }

        internal Product(decimal value)
        {
            if (IsValid(value))
            {
                Value = value;
            }
            else
            {
                throw new InvalidProductException($"{value:0.##} is an invalid product value.");
            }
        }

        public static Product operator *(Product a, Product b) => new Product(a.Value * b.Value) ;


        public Product Round()
        {
            var roundedValue = Math.Round(Value);
            return new Product(roundedValue);
        }

        public override string ToString()
        {
            return $"{Value:0.##}";
        }

        public static Option<Product> TryParseProduct(decimal numericProduct)
        {
            if (IsValid(numericProduct))
            {
                return Some<Product>(new(numericProduct));
            }
            else
            {
                return None;
            }
        }

        public static Option<Product> TryParseProduct(string productString)
        {
            if(decimal.TryParse(productString, out decimal numericProduct) && IsValid(numericProduct))
            {
                return Some<Product>(new(numericProduct));
            }
            else
            {
                return None;
            }
        }

        private static bool IsValid(decimal numericProduct) => numericProduct > 0;
    }
}
