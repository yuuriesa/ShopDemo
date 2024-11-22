using CustomerManagement.DTO;
using CustomerManagement.Models;
using CustomerManagement.Repository;

namespace CustomerManagement.Services
{
    public class CustomerServices : ICustomerServices
    {
        private ICustomerRepository _repository;

        public CustomerServices(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public List<string> GetDuplicateEmails(IEnumerable<CustomerDto> customers)
        {
            return customers
            .GroupBy(c => c.Email)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();
        }

        public IEnumerable<Customer> GetAll(PaginationFilter validFilter)
        {
            var allCustomers = _repository.GetAll(validFilter);
            return allCustomers;
        }

        public Customer GetById(int id)
        {
            var findCustomer = _repository.GetById(id);
            return findCustomer;
        }
    }
}