using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using api_basics.Daos;
using api_basics.Models;

namespace api_basics.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductContext _context;

        public ProductsController(ProductContext context)
        {
            _context = context;

            if (_context.Products.Any()) return;

            ProductSeed.InitData(context);
        }

        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IQueryable<Product>> GetProducts([FromQuery] string department)
        {
            var result = _context.Products as IQueryable<Product>;

            if (!string.IsNullOrEmpty(department))
            {
                result = result.Where(p => p.Department.StartsWith(department, StringComparison.InvariantCultureIgnoreCase));
            }

            return Ok(result
                .OrderBy(p => p.ProductNumber)
                .Take(15));
        }

        [HttpGet]
        [Route("{productNumber}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Product> GetProductByProductNumber([FromRoute]
            string productNumber)
        {
            var productDb = _context.Products
              .FirstOrDefault(p => p.ProductNumber.Equals(productNumber,
                        StringComparison.InvariantCultureIgnoreCase));

            if (productDb == null) return NotFound();

            return Ok(productDb);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Product> PostProduct([FromBody] Product product)
        {
            try
            {
                _context.Products.Add(product);
                _context.SaveChanges();

                return new CreatedResult($"/products/{product.ProductNumber.ToLower()}", product);
            }
            catch (Exception e)
            {
                // Typically an error log is produced here
                return ValidationProblem(e.Message);
            }
        }

        [HttpPut]
        [Route("{productNumber}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Product> PutProduct([FromRoute] string productNumber, [FromBody] Product newProduct)
        {
            try
            {
                var productList = _context.Products as IQueryable<Product>;
                var product = productList.First(p => p.ProductNumber.Equals(productNumber));

                _context.Products.Remove(product);
                _context.Products.Add(newProduct);
                _context.SaveChanges();

                return new CreatedResult($"/products/{product.ProductNumber.ToLower()}", product);
            }
            catch (Exception e)
            {
                // Typically an error log is produced here
                return ValidationProblem(e.Message);
            }
        }

        [HttpDelete]
        [Route("{productNumber}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Product> DeleteProduct([FromRoute] string productNumber)
        {
            try
            {
                var productList = _context.Products as IQueryable<Product>;
                var product = productList.First(p => p.ProductNumber.Equals(productNumber));

                _context.Products.Remove(product);
                _context.SaveChanges();

                return new CreatedResult($"/products/{product.ProductNumber.ToLower()}", product);
            }
            catch (Exception e)
            {
                // Typically an error log is produced here
                return ValidationProblem(e.Message);
            }
        }

        [HttpPatch]
        [Route("{productNumber}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Product> PatchProduct([FromRoute] string productNumber, [FromBody] ProductPatch newProduct)
        {
            try
            {
                var productList = _context.Products as IQueryable<Product>;
                var product = productList.First(p => p.ProductNumber.Equals(productNumber));

                product.ProductNumber = newProduct.ProductNumber ?? product.ProductNumber;
                product.Department = newProduct.Department ?? product.Department;
                product.Name = newProduct.Name ?? product.Name;
                product.Price = newProduct.Price ?? product.Price;
                product.RelatedProducts = newProduct.RelatedProducts ?? product.RelatedProducts;


                _context.Products.Update(product);
                _context.SaveChanges();

                return new CreatedResult($"/products/{product.ProductNumber.ToLower()}", product);
            }
            catch (Exception e)
            {
                // Typically an error log is produced here
                return ValidationProblem(e.Message);
            }
        }

        [HttpPatch]
        [Route("{productNumber}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Product> AddRelatedProduct([FromRoute] string productNumber, [FromBody] RelatedProduct relatedProduct)
        {
            try
            {
                var productList = _context.Products as IQueryable<Product>;
                var product = productList.First(p => p.ProductNumber.Equals(productNumber));

                product.RelatedProducts.Add(relatedProduct);

                _context.Products.Update(product);
                _context.SaveChanges();

                return new CreatedResult($"/products/{product.ProductNumber.ToLower()}", product);
            }
            catch (Exception e)
            {
                // Typically an error log is produced here
                return ValidationProblem(e.Message);
            }
        }
    }
}

