using CustomerManagement.Data;
using CustomerManagement.DTO;
using CustomerManagement.Models;
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

            return Created("", getProductByCodeForResponse);
        }
    }
}