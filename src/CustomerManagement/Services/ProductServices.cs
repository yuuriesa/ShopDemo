using CustomerManagement.DTO;
using CustomerManagement.Models;
using CustomerManagement.Repository;
using CustomerManagement.Utils;

namespace CustomerManagement.Services
{
    public class ProductServices : IProductServices
    {
        private IProductRepository _productRepository;

        public ProductServices(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public ServiceResult<Product> Add(ProductDtoRequest product)
        {
            var newProduct = Product.RegisterNew(code: product.Code, name: product.Name);

            if (!newProduct.IsValid)
            {
                return ServiceResult<Product>.ErrorResult(ResponseMessagesCustomers.FieldsAreInvalidProduct, 400);
            }

            _productRepository.Add(newProduct);

            return ServiceResult<Product>.SuccessResult(newProduct, 201);
        }

        public IEnumerable<Product> GetAll(PaginationFilter paginationFilter)
        {
            return _productRepository.GetAll(paginationFilter);
        }

        public ProductDtoResponse GetByCode(string code)
        {
            var findProductByCode = _productRepository.GetByCode(code);
            return findProductByCode;
        }
    }
}