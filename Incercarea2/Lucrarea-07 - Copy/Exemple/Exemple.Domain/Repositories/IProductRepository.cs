using Exemple.Domain.Models;
using LanguageExt;
using System.Collections.Generic;
using static Exemple.Domain.Models.Products;

namespace Exemple.Domain.Repositories
{
    public interface IProductRepository
    {
        TryAsync<List<CalculatedCustomerProduct>> TryGetExistingProduct();

        TryAsync<Unit> TrySaveProduct(PublishedProducts product);
    }
}
