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
            var productWithCodeExists = GetByCode(product.Code);

            if (productWithCodeExists != null)
            {
                return ServiceResult<Product>.ErrorResult(ResponseMessagesCustomers.ProductWithThisCodeExists, 422);
            }

            var newProduct = Product.RegisterNew(code: product.Code, name: product.Name);

            if (!newProduct.IsValid)
            {
                return ServiceResult<Product>.ErrorResult(ResponseMessagesCustomers.FieldsAreInvalidProduct, 400);
            }

            _productRepository.Add(newProduct);

            return ServiceResult<Product>.SuccessResult(newProduct, 201);
        }

        public IEnumerable<ProductDtoResponse> GetAll(PaginationFilter paginationFilter)
        {
            var allProducts = _productRepository.GetAll(paginationFilter);
            return from product in allProducts
                   select new ProductDtoResponse
                   {
                        Id = product.Id,
                        Code = product.Code,
                        Name = product.Name
                   };
        }

        public ProductDtoResponse GetByCode(string code)
        {
            var findProductByCode = _productRepository.GetByCode(code);
            if (findProductByCode == null)
            {
                return null!;
            }
            return findProductByCode;
        }

        public ProductDtoResponse GetById(int id)
        {
            var findProductById = _productRepository.GetById(id);
            if (findProductById == null)
            {
                return null!;
            }

            //var product = Product.SetExistingInfo(id: findProductById.Id, code: findProductById.Code, name: findProductById.Name);
            var productDtoResponse = new ProductDtoResponse
            {
                Id = findProductById.Id,
                Code = findProductById.Code,
                Name = findProductById.Name
            };
            return productDtoResponse;
        }
    }
}