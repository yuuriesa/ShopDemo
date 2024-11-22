using CustomerManagement.DTO;
using CustomerManagement.Models;

namespace CustomerManagement.Services
{
    public interface ICustomerServices
    {
        public List<string> GetDuplicateEmails(IEnumerable<CustomerDto> customers);
        public IEnumerable<Customer> GetAll(PaginationFilter validFilter);
        public Customer GetById(int id);
        public bool VerifyDateOfBirth(DateTime customerDateOfBirth);
        public Customer Add(CustomerDto customer);
        public Customer GetByEmail(string email);
    }
}