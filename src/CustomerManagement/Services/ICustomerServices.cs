using CustomerManagement.DTO;
using CustomerManagement.Models;

namespace CustomerManagement.Services
{
    public interface ICustomerServices
    {
        public List<string> GetDuplicateEmails(IEnumerable<CustomerDto> customers);
        public IEnumerable<Customer> GetAll(PaginationFilter validFilter);
        public Customer GetById(int id);
        public IEnumerable<Customer> GetListCustomersByEmail(IEnumerable<CustomerDto> customers);
        public bool VerifyDateOfBirth(DateTime customerDateOfBirth);
        public Customer Add(CustomerDto customer);
        public void AddRange(IEnumerable<CustomerDto> customers);
        public Customer GetByEmail(string email);
        public void SaveChanges();
    }
}