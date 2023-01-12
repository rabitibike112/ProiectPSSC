using Exemple.Domain.Models;
using static LanguageExt.Prelude;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Exemple.Domain.Models.Products;
using System.Threading.Tasks;

namespace Exemple.Domain
{
    public static class ProductsOperation
    {
        public static Task<IProducts> ValidateProducts(Func<CustomerRegistrationNumber, Option<CustomerRegistrationNumber>> checkCustomerExists, UnvalidatedProducts Products) =>
            Products.ProductList
                      .Select(ValidateCustomerProduct(checkCustomerExists))
                      .Aggregate(CreateEmptyValatedProductsList().ToAsync(), ReduceValidProducts)
                      .MatchAsync(
                            Right: validatedProduct => new ValidatedProducts(validatedProduct),
                            LeftAsync: errorMessage => Task.FromResult((IProducts)new InvalidProducts(Products.ProductList, errorMessage))
                      );

        private static Func<UnvalidatedCustomerProduct, EitherAsync<string, ValidatedCustomerProduct>> ValidateCustomerProduct(Func<CustomerRegistrationNumber, Option<CustomerRegistrationNumber>> checkCustomerExists) =>
            unvalidatedCustomerProduct => ValidateCustomerProduct(checkCustomerExists, unvalidatedCustomerProduct);

        private static EitherAsync<string, ValidatedCustomerProduct> ValidateCustomerProduct(Func<CustomerRegistrationNumber, Option<CustomerRegistrationNumber>> checkCustomerExists, UnvalidatedCustomerProduct unvalidatedProduct)=>
            from productPrice in Product.TryParseProduct(unvalidatedProduct.ProductPrice)
                                   .ToEitherAsync($"Invalid Price ({unvalidatedProduct.CustomerRegistrationNumber}, {unvalidatedProduct.ProductPrice})")
            from productAmount in Product.TryParseProduct(unvalidatedProduct.ProductAmount)
                                   .ToEitherAsync($"Invalid Amount ({unvalidatedProduct.CustomerRegistrationNumber}, {unvalidatedProduct.ProductAmount})")
            from CustomerRegistrationNumber in CustomerRegistrationNumber.TryParse(unvalidatedProduct.CustomerRegistrationNumber)
                                   .ToEitherAsync($"Invalid customer registration number ({unvalidatedProduct.CustomerRegistrationNumber})")
            from productExists in checkCustomerExists(CustomerRegistrationNumber)
                                   .ToEitherAsync($"Customer {CustomerRegistrationNumber.Value} does not exist.")
            select new ValidatedCustomerProduct(CustomerRegistrationNumber, productPrice, productAmount);

        private static Either<string, List<ValidatedCustomerProduct>> CreateEmptyValatedProductsList() =>
            Right(new List<ValidatedCustomerProduct>());

        private static EitherAsync<string, List<ValidatedCustomerProduct>> ReduceValidProducts(EitherAsync<string, List<ValidatedCustomerProduct>> acc, EitherAsync<string, ValidatedCustomerProduct> next) =>
            from list in acc
            from nextProduct in next
            select list.AppendValidProduct(nextProduct);

        private static List<ValidatedCustomerProduct> AppendValidProduct(this List<ValidatedCustomerProduct> list, ValidatedCustomerProduct validProduct)
        {
            list.Add(validProduct);
            return list;
        }

        public static IProducts CalculateFinalProducts(IProducts Products) => Products.Match(
            whenUnvalidatedProducts: unvalidatedPrice => unvalidatedPrice,
            whenInvalidProducts: invalidPrice => invalidPrice,
            whenFailedProducts: failedPrice => failedPrice, 
            whenCalculatedProducts: calculatedPrice => calculatedPrice,
            whenPublishedProducts: publishedPrice => publishedPrice,
            whenValidatedProducts: CalculateFinalProduct
        );

        private static IProducts CalculateFinalProduct(ValidatedProducts validProducts) =>
            new CalculatedProducts(validProducts.ProductList
                                                    .Select(CalculateCustomerFinalProduct)
                                                    .ToList()
                                                    .AsReadOnly());

        private static CalculatedCustomerProduct CalculateCustomerFinalProduct(ValidatedCustomerProduct validProduct) => 
            new CalculatedCustomerProduct(validProduct.CustomerRegistrationNumber,
                                      validProduct.ProductPrice,
                                      validProduct.ProductAmount,
                                      validProduct.ProductPrice * validProduct.ProductAmount);

        public static IProducts MergeProducts(IProducts Products, IEnumerable<CalculatedCustomerProduct> existingProducts) => Products.Match(
            whenUnvalidatedProducts: unvalidaTedPrice => unvalidaTedPrice,
            whenInvalidProducts: invalidPrice => invalidPrice,
            whenFailedProducts: failedPrice => failedPrice,
            whenValidatedProducts: validatedPrice => validatedPrice,
            whenPublishedProducts: publishedPrice => publishedPrice,
            whenCalculatedProducts: calculatedPrice => MergeProducts(calculatedPrice.ProductList, existingProducts));

        private static CalculatedProducts MergeProducts(IEnumerable<CalculatedCustomerProduct> newList, IEnumerable<CalculatedCustomerProduct> existingList)
        {
            var updatedAndNewProducts = newList.Select(Product => Product with { ProductId = existingList.FirstOrDefault(g => g.CustomerRegistrationNumber == Product.CustomerRegistrationNumber)?.ProductId ?? 0, IsUpdated = true });
            var oldProducts = existingList.Where(Product => !newList.Any(g => g.CustomerRegistrationNumber == Product.CustomerRegistrationNumber));
            var allProducts = updatedAndNewProducts.Union(oldProducts)
                                               .ToList()
                                               .AsReadOnly();
            return new CalculatedProducts(allProducts);
        }

        public static IProducts PublishProducts(IProducts Products) => Products.Match(
            whenUnvalidatedProducts: unvalidaTedPrice => unvalidaTedPrice,
            whenInvalidProducts: invalidPrice => invalidPrice,
            whenFailedProducts: failedPrice => failedPrice,
            whenValidatedProducts: validatedPrice => validatedPrice,
            whenPublishedProducts: publishedPrice => publishedPrice,
            whenCalculatedProducts: GenerateExport);

        private static IProducts GenerateExport(CalculatedProducts calculatedPrice) => 
            new PublishedProducts(calculatedPrice.ProductList, 
                                    calculatedPrice.ProductList.Aggregate(new StringBuilder(), CreateCsvLine).ToString(), 
                                    DateTime.Now);

        private static StringBuilder CreateCsvLine(StringBuilder export, CalculatedCustomerProduct Product) =>
            export.AppendLine($"{Product.CustomerRegistrationNumber.Value}, {Product.ProductPrice}, {Product.ProductAmount}, {Product.FinalTotal}");
    }
}
