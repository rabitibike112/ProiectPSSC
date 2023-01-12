using Exemple.Domain.Models;
using Exemple.Domain.Repositories;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Example.Data.Repositories
{
    public class CustomersRepository: ICustomerRepository
    {
        private readonly ProductContext orderContext;

        public CustomersRepository(ProductContext orderContext)
        {
            this.orderContext = orderContext;  
        }

        public TryAsync<List<CustomerRegistrationNumber>> TryGetExistingCustomer(IEnumerable<string> customersToCheck) => async () =>
        {
            var customers = await orderContext.Customers
                                              .Where(customer => customersToCheck.Contains(customer.RegistrationNumber))
                                              .AsNoTracking()
                                              .ToListAsync();
            return customers.Select(customer => new CustomerRegistrationNumber(customer.RegistrationNumber))
                           .ToList();
        };
    }
}
