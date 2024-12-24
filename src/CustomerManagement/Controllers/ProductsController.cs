using CustomerManagement.Data;
using CustomerManagement.DTO;
using CustomerManagement.Models;
using CustomerManagement.Repository;
using CustomerManagement.Services;
using CustomerManagement.Utils;
using Microsoft.AspNetCore.Mvc;

namespace CustomerManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IProductRepository _productRepository;
        private readonly IProductServices _productServices;

        public ProductsController(IProductServices productServices, ApplicationDbContext dbContext)
        {
            _productServices = productServices;
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAll(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 0 || pageSize < 0) return BadRequest(ResponseMessagesCustomers.CustomerPaginationError);

            var validFilter = new PaginationFilter(pageNumber: pageNumber, pageSize: pageSize);
            var allProducts = _productServices.GetAll(validFilter);
            if (allProducts.Count() == 0) return NoContent();

            return Ok(allProducts);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var product = _productServices.GetById(id);

            if (product == null)
            {
                return NotFound(ResponseMessagesCustomers.ProductNotFoundMessage);
            }

            return Ok(product);
        }

        [HttpPost]
        public IActionResult Add(ProductDtoRequest productDtoRequest)
        {
            var result = _productServices.Add(productDtoRequest);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Message);
            }

            _dbContext.SaveChanges();

            var getProductByCodeForResponse = _productServices.GetByCode(result.Data.Code);

            return CreatedAtAction(actionName: nameof(GetById) , routeValues: new { id = result.Data.Id }, getProductByCodeForResponse);
        }

        [HttpPost("batch")]
        public async Task<IActionResult> AddBatchProducts([FromBody] IEnumerable<ProductDtoRequest> productsRequest)
        {
            if (productsRequest.Count() == 0)
            {
                return NoContent();
            }

            var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var result = _productServices.AddBatchProducts(productsRequest);

                if (!result.Success)
                {
                    return StatusCode(statusCode: result.StatusCode, value: result.Message);
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                List<ProductDtoResponse> listProductsDtoResponse = new List<ProductDtoResponse>();

                foreach (var product in result.Data)
                {
                    var newProductDtoResponse = _productServices.GetByCode(code: product.Code);
                    listProductsDtoResponse.Add(newProductDtoResponse);
                }

                return Created("", listProductsDtoResponse);
            }
            catch (Exception err)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, err.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] ProductDtoRequest productRequest)
        {
            var result = _productServices.UpdateProduct(id: id, productRequest: productRequest);
            if (!result.Success)
            {
                return StatusCode(statusCode: result.StatusCode, value: result.Message);
            }

            _dbContext.SaveChanges();

            var newProductDtoResponse = _productServices.GetByCode(code: result.Data.Code);

            return Ok(newProductDtoResponse);
        }

        [HttpPatch("{id}")]
        public IActionResult UpdatePatchProduct(int id, [FromBody] ProductPatchDtoRequest productPatchDtoRequest)
        {
            var result = _productServices.UpdatePatchProduct(id: id, productPatchDtoRequest: productPatchDtoRequest);

            if (!result.Success)
            {
                return StatusCode(statusCode: result.StatusCode, value: result.Message);
            }

            _dbContext.SaveChanges();

            var getProductByCodeForResponse = _productServices.GetByCode(result.Data.Code);

            return Ok(getProductByCodeForResponse);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _productServices.Delete(id: id);

            if (!result.Success)
            {
                return StatusCode(statusCode: result.StatusCode, value: result.Message);
            }

            _dbContext.SaveChanges();

            return NoContent();
        }
    }
}