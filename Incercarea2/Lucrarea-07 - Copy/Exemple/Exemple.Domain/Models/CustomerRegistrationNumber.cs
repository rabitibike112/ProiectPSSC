using LanguageExt;
using static LanguageExt.Prelude;
using System.Text.RegularExpressions;

namespace Exemple.Domain.Models
{
    public record CustomerRegistrationNumber
    {
        public const string Pattern = "^RO[0-9]{5}$";
        private static readonly Regex PatternRegex = new(Pattern);

        public string Value { get; }

        internal CustomerRegistrationNumber(string value)
        {
            if (IsValid(value))
            {
                Value = value;
            }
            else
            {
                throw new InvalidCustomerRegistrationNumberException("");
            }
        }

        private static bool IsValid(string stringValue) => PatternRegex.IsMatch(stringValue);

        public override string ToString()
        {
            return Value;
        }

        public static Option<CustomerRegistrationNumber> TryParse(string stringValue)
        {
            if (IsValid(stringValue))
            {
                return Some<CustomerRegistrationNumber>(new(stringValue));
            }
            else
            {
                return None;
            }
        }
    }
}
