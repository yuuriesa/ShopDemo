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

        public ServiceResult<IEnumerable<Product>> AddBatchProducts(IEnumerable<ProductDtoRequest> products)
        {
            List<Product> listProducts = new List<Product>();

            var duplicateCodesInProduct = products
                .GroupBy(p => p.Code)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key);

            if (duplicateCodesInProduct.Any())
            {
                return ServiceResult<IEnumerable<Product>>
                    .ErrorResult
                    (
                        $"{ResponseMessagesCustomers.DuplicateCodesInInputBatchProduct}: {string.Join(", ", duplicateCodesInProduct)}", 400
                    );
            }

            foreach (var product in products)
            {
                var productWithCodeExists = GetByCode(product.Code);

                if (productWithCodeExists != null)
                {
                    return ServiceResult<IEnumerable<Product>>.ErrorResult
                    (
                        ResponseMessagesCustomers.ProductWithThisCodeExists + $": {product.Code}", 422
                    );
                }

                var newProduct = Product.RegisterNew(code: product.Code, name: product.Name);

                if (!newProduct.IsValid)
                {
                    return ServiceResult<IEnumerable<Product>>.ErrorResult
                    (
                        ResponseMessagesCustomers.FieldsAreInvalidProduct, 400
                    );
                }

                listProducts.Add(newProduct);
            }

            _productRepository.AddRange(listProducts);

            return ServiceResult<IEnumerable<Product>>.SuccessResult(listProducts, 201);
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

            var product = Product.SetExistingInfo(id: findProductById.Id, code: findProductById.Code, name: findProductById.Name);
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