using CustomerManagement.Data;
using CustomerManagement.Models;

namespace CustomerManagement.Repository
{
    public class AddressRepository : RepositoryBase<Address>, IAddressRepository
    {
        private IApplicationDbContext _dbContext;
        public AddressRepository(ApplicationDbContext context) : base(context)
        {
            _dbContext = context;
        }

        public IEnumerable<Address> GetAllAddressesByIdCustomer(int customerId)
        {
            var addresses = _dbContext.Addresses.Where(a => a.CustomerId == customerId);
            return addresses;
        }
    }
}