using CustomerManagement.DTO;
using CustomerManagement.Models;
using CustomerManagement.Utils;

namespace CustomerManagement.Services
{
    public interface IProductServices
    {
        public IEnumerable<ProductDtoResponse> GetAll(PaginationFilter paginationFilter);
        public ProductDtoResponse GetByCode(string code);
        public ProductDtoResponse GetById(int id);
        public ServiceResult<Product> Add(ProductDtoRequest product);
        public ServiceResult<IEnumerable<Product>> AddBatchProducts(IEnumerable<ProductDtoRequest> products);
        public ServiceResult<Product> UpdateProduct(int id, ProductDtoRequest productRequest);
    }
}