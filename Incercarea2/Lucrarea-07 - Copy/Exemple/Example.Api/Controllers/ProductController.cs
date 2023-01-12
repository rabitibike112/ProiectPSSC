using Exemple.Domain;
using Exemple.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System;
using Example.Api.Models;
using Exemple.Domain.Models;

namespace Example.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private ILogger<ProductController> logger;

        public ProductController(ILogger<ProductController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProduct([FromServices] IProductRepository productRepository) =>
            await productRepository.TryGetExistingProduct().Match(
               Succ: GetAllProductHandleSuccess,
               Fail: GetAllProductHandleError
            );

        private ObjectResult GetAllProductHandleError(Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return base.StatusCode(StatusCodes.Status500InternalServerError, "UnexpectedError");
        }

        private OkObjectResult GetAllProductHandleSuccess(List<Exemple.Domain.Models.CalculatedCustomerProduct> prod) =>
        Ok(prod.Select(prodd => new
        {
            CustomerRegistrationNumber = prodd.CustomerRegistrationNumber.Value,
            prodd.ProductPrice,
            prodd.ProductAmount,
            prodd.FinalTotal
        }));

        [HttpPost]
        public async Task<IActionResult> PublishProduct([FromServices]PublishOrderWorkflow publishProductWorkflow, [FromBody]InputProduct[] prod)
        {
            var unvalidatedProduct = prod.Select(MapInputProductToUnvalidatedProduct)
                                          .ToList()
                                          .AsReadOnly();
            PublishProductCommand command = new(unvalidatedProduct);
            var result = await publishProductWorkflow.ExecuteAsync(command);
            return result.Match<IActionResult>(
                whenProductsPublishFaildEvent: failedEvent => StatusCode(StatusCodes.Status500InternalServerError, failedEvent.Reason),
                whenProductsPublishScucceededEvent: successEvent => Ok()
            );
        }

        private static UnvalidatedCustomerProduct MapInputProductToUnvalidatedProduct(InputProduct product) => new UnvalidatedCustomerProduct(
            CustomerRegistrationNumber: product.RegistrationNumber,
            ProductPrice: product.Price,
            ProductAmount: product.Amount);
    }
}
