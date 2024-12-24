using CustomerManagement.Data;
using CustomerManagement.DTO;
using CustomerManagement.Models;
using CustomerManagement.Repository;
using CustomerManagement.Utils;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Services
{
    public class ProductServices : IProductServices
    {
        private IProductRepository _productRepository;
        private ApplicationDbContext _dbContext;

        public ProductServices(IProductRepository productRepository, ApplicationDbContext dbContext)
        {
            _productRepository = productRepository;
            _dbContext = dbContext;
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

        public ServiceResult<Product> UpdatePatchProduct(int id, ProductPatchDtoRequest productPatchDtoRequest)
        {
            //Usei o AsNoTracking() ao consultar os dados. Isso evita que a entidade seja rastreada, permitindo que eu substitua ou atualize com uma nova instância sem gerar erros.
            var findProductById = _dbContext.Products.AsNoTracking().FirstOrDefault(p => p.Id == id);

            if (findProductById == null)
            {
                return ServiceResult<Product>.ErrorResult(ResponseMessagesCustomers.ProductNotFoundMessage, 404);
            }

            if (productPatchDtoRequest.Name != null)
            {
                var updatedProduct = Product.SetExistingInfo(id: findProductById.Id, code: findProductById.Code, name: productPatchDtoRequest.Name);
                
                if (!updatedProduct.IsValid)
                {
                    return ServiceResult<Product>.ErrorResult(ResponseMessagesCustomers.FieldsAreInvalidProduct, 422);
                }
                
                _productRepository.Update(id: id, entity: updatedProduct);
            };

            return ServiceResult<Product>.SuccessResult(findProductById);
        }

        public ServiceResult<Product> UpdateProduct(int id, ProductDtoRequest productRequest)
        {
            //Usei o AsNoTracking() ao consultar os dados. Isso evita que a entidade seja rastreada, permitindo que eu substitua ou atualize com uma nova instância sem gerar erros.
            var findProductById = _dbContext.Products.AsNoTracking().FirstOrDefault(p =>  p.Id == id);
            if (findProductById == null)
            {
                return ServiceResult<Product>.ErrorResult(message: ResponseMessagesCustomers.ProductNotFoundMessage, statusCode: 404);
            }

            //Preencher nova instancia com o id do estado do banco de dados atual com os novos dados
            var setCurrentProduct = Product.SetExistingInfo(id: findProductById.Id, code: productRequest.Code, name: productRequest.Name);

            if (!setCurrentProduct.IsValid)
            {
                return ServiceResult<Product>.ErrorResult(message: ResponseMessagesCustomers.FieldsAreInvalidProduct, statusCode: 400);
            }

            _productRepository.Update(id: id, entity: setCurrentProduct);

            return ServiceResult<Product>.SuccessResult(data: setCurrentProduct);
        }        
    }
}