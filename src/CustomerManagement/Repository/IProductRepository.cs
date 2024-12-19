using CustomerManagement.DTO;
using CustomerManagement.Models;
using CustomerManagement.Utils;

namespace CustomerManagement.Repository
{
    public interface IProductRepository : IRepositoryBase<Product>
    {
        public ProductDtoResponse? GetByCode(string code);
    }
}