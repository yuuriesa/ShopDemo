using CustomerManagement.Models;

namespace CustomerManagement.Repository
{
    public interface ICustomerRepository : IRepositoryBase<Customer>
    {
        public Customer GetByEmail(string email);
    }
}