using CustomerManagement.Data;
using CustomerManagement.DTO;
using CustomerManagement.Models;

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
            .AsEnumerable() // Força execução no cliente
            .FirstOrDefault(p => p.Code == code);

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