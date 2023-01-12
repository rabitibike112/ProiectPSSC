using Exemple.Domain.Models;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Domain.Repositories
{
    public interface ICustomerRepository
    {
        TryAsync<List<CustomerRegistrationNumber>> TryGetExistingCustomer(IEnumerable<string> customerToCheck);
    }
}
