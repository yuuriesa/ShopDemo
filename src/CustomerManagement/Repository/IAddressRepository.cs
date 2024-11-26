using CustomerManagement.Models;

namespace CustomerManagement.Repository
{
    public interface IAddressRepository : IRepositoryBase<Address>
    {
        public IEnumerable<Address> GetAllAddressesByIdCustomer(int customerId);
    }
}