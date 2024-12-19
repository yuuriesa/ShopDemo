using CustomerManagement.Data;
using CustomerManagement.DTO;
using CustomerManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Repository
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        private readonly IApplicationDbContext _dbContext;
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _dbContext = context;
        }

        public ProductDtoResponse? GetByCode(string code)
        {
            var findProductByCode = _dbContext.Products
           .Where(p => EF.Property<string>(p, "_code") == code).FirstOrDefault();

            if (findProductByCode == null)
            {
                return null!;
            }

            var product = Product.SetExistingInfo(code: findProductByCode.Code, name: findProductByCode.Name);
            var productDtoResponse = new ProductDtoResponse
            {
                Code = product.Code,
                Name = product.Name
            };
            return productDtoResponse;
        }
    }
}