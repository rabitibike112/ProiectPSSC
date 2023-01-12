using Exemple.Domain.Models;
using static Exemple.Domain.Models.ProductsPublishedEvent;
using static Exemple.Domain.ProductsOperation;
using System;
using static Exemple.Domain.Models.Products;
using LanguageExt;
using System.Threading.Tasks;
using System.Collections.Generic;
using Exemple.Domain.Repositories;
using System.Linq;
using static LanguageExt.Prelude;
using Microsoft.Extensions.Logging;
using Example.Events;
using Example.Dto.Events;
using Example.Dto.Models;

namespace Exemple.Domain
{
    public class PublishOrderWorkflow
    {
        private readonly ICustomerRepository customerRepository;
        private readonly IProductRepository productRepository;
        private readonly ILogger<PublishOrderWorkflow> logger;
        private readonly IEventSender eventSender;

        public PublishOrderWorkflow(ICustomerRepository customerRepository, IProductRepository productRepository,
                                    ILogger<PublishOrderWorkflow> logger, IEventSender eventSender)
        {
            this.customerRepository = customerRepository;
            this.productRepository = productRepository;
            this.logger = logger;
            this.eventSender = eventSender;
        }

        public async Task<IProductsPublishedEvent> ExecuteAsync(PublishProductCommand command)
        {
            UnvalidatedProducts unvalidatedProduct = new UnvalidatedProducts(command.InputProduct);

            var result = from customers in customerRepository.TryGetExistingCustomer(unvalidatedProduct.ProductList.Select(product => product.CustomerRegistrationNumber))
                                          .ToEither(ex => new FailedProducts(unvalidatedProduct.ProductList, ex) as IProducts)
                         from existingProduct in productRepository.TryGetExistingProduct()
                                          .ToEither(ex => new FailedProducts(unvalidatedProduct.ProductList, ex) as IProducts)
                         let checkCustomerExists = (Func<CustomerRegistrationNumber, Option<CustomerRegistrationNumber>>)(customer => CheckCustomerExists(customers, customer))
                         from publishedProduct in ExecuteWorkflowAsync(unvalidatedProduct, existingProduct, checkCustomerExists).ToAsync()
                         from saveResult in productRepository.TrySaveProduct(publishedProduct)
                                          .ToEither(ex => new FailedProducts(unvalidatedProduct.ProductList, ex) as IProducts)
                         let Product = publishedProduct.ProductList.Select(product => new PublishedCustomerProduct(
                                                        product.CustomerRegistrationNumber,
                                                        ProductPrice: product.ProductPrice,
                                                        ProductAmount: product.ProductAmount,
                                                        FinalTotal: product.FinalTotal))
                         let successfulEvent = new ProductsPublishScucceededEvent(Product, publishedProduct.PublishedDate)
                         let eventToPublish = new ProductPublishedEvent()
                         {
                             Product = Product.Select(g=>new CustomerProductDto()
                             {
                                 Name = g.CustomerRegistrationNumber.Value, 
                                 CustomerRegistrationNumber = g.CustomerRegistrationNumber.Value,
                                 ProductAmount = g.ProductAmount.Value,
                                 ProductPrice = g.ProductPrice.Value,
                                 FinalTotal = g.FinalTotal.Value
                                 
                             }).ToList()
                         }
                         from publishEventResult in eventSender.SendAsync("", eventToPublish)
                                              .ToEither(ex => new FailedProducts(unvalidatedProduct.ProductList, ex) as IProducts)
                         select successfulEvent;

            return await result.Match(
                    Left: Products => GenerateFailedEvent(Products) as IProductsPublishedEvent,
                    Right: publishedProduct => publishedProduct
                );
        }

        private async Task<Either<IProducts, PublishedProducts>> ExecuteWorkflowAsync(UnvalidatedProducts unvalidatedProduct,
                                                                                          IEnumerable<CalculatedCustomerProduct> existingProduct,
                                                                                          Func<CustomerRegistrationNumber, Option<CustomerRegistrationNumber>> checkCustomerExists)
        {

            IProducts product = await ValidateProducts(checkCustomerExists, unvalidatedProduct);
            product = CalculateFinalProducts(product);
            product = MergeProducts(product, existingProduct);
            product = PublishProducts(product);

            return product.Match<Either<IProducts, PublishedProducts>>(
                whenUnvalidatedProducts: unvalidatedProduct => Left(unvalidatedProduct as IProducts),
                whenCalculatedProducts: calculatedProduct => Left(calculatedProduct as IProducts),
                whenInvalidProducts: invalidProduct => Left(invalidProduct as IProducts),
                whenFailedProducts: failedProduct => Left(failedProduct as IProducts),
                whenValidatedProducts: validatedProduct => Left(validatedProduct as IProducts),
                whenPublishedProducts: publishedProduct => Right(publishedProduct)
            );
        }

        private Option<CustomerRegistrationNumber> CheckCustomerExists(IEnumerable<CustomerRegistrationNumber> customer, CustomerRegistrationNumber customerRegistrationNumber)
        {
            if (customer.Any(c => c == customerRegistrationNumber))
            {
                return Some(customerRegistrationNumber);
            }
            else
            {
                return None;
            }
        }

        private ProductsPublishFaildEvent GenerateFailedEvent(IProducts Products) =>
            Products.Match<ProductsPublishFaildEvent>(
                whenUnvalidatedProducts: unvalidatedProducts => new($"Invalid state {nameof(UnvalidatedProducts)}"),
                whenInvalidProducts: invalidProducts => new(invalidProducts.Reason),
                whenValidatedProducts: validatedProducts => new($"Invalid state {nameof(ValidatedProducts)}"),
                whenFailedProducts: failedProducts =>
                {
                    logger.LogError(failedProducts.Exception, failedProducts.Exception.Message);
                    return new(failedProducts.Exception.Message);
                },
                whenCalculatedProducts: calculatedProducts => new($"Invalid state {nameof(CalculatedProducts)}"),
                whenPublishedProducts: publishedProducts => new($"Invalid state {nameof(PublishedProducts)}"));
    }
}
