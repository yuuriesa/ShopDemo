using CustomerManagement.DTO;
using CustomerManagement.Models;
using CustomerManagement.Utils;

namespace CustomerManagement.Services
{
    public interface ICustomerServices
    {
        public List<string> GetDuplicateEmails(IEnumerable<CustomerDto> customers);
        public IEnumerable<Customer> GetAll(PaginationFilter validFilter);
        public Customer GetById(int id);
        public Customer GetByEmail(string email);
        public IEnumerable<Customer> GetListCustomersByEmail(IEnumerable<CustomerDto> customers);
        public bool VerifyDateOfBirth(DateTime customerDateOfBirth);
        public Customer Add(CustomerDto customer);
        public void AddRange(IEnumerable<CustomerDto> customers);
        public ServiceResult<Customer> Update(int id,  CustomerDto customerDto);
        public void UpdatePatch(int id, Customer findCustomer, CustomerPatchDto customerPatchDto);
        public void SaveChanges();
    }
}