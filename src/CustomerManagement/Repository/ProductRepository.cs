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
           .FirstOrDefault(p => EF.Property<string>(p, "_code") == code);

            if (findProductByCode == null)
            {
                return null!;
            }

            var product = Product.SetExistingInfo(id: findProductByCode.Id, code: findProductByCode.Code, name: findProductByCode.Name);
            var productDtoResponse = new ProductDtoResponse
            {
                Id = product.Id,
                Code = product.Code,
                Name = product.Name
            };
            return productDtoResponse;
        }
    }
}