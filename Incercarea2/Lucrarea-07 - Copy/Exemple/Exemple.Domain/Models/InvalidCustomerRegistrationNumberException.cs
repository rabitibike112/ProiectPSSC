using System;
using System.Runtime.Serialization;

namespace Exemple.Domain.Models
{
    [Serializable]
    internal class InvalidCustomerRegistrationNumberException : Exception
    {
        public InvalidCustomerRegistrationNumberException()
        {
        }

        public InvalidCustomerRegistrationNumberException(string? message) : base(message)
        {
        }

        public InvalidCustomerRegistrationNumberException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidCustomerRegistrationNumberException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}