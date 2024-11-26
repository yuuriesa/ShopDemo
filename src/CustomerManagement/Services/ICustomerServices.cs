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
        public ServiceResult<Customer> Add(CustomerDto customer);
        public ServiceResult<IEnumerable<Customer>> AddRange(IEnumerable<CustomerDto> customers);
        public ServiceResult<Customer> Update(int id,  CustomerDto customerDto);
        public ServiceResult<Customer> UpdatePatch(int id, CustomerPatchDto customerPatchDto);
        public ServiceResult<Customer> Delete(int id);
        public CustomerDtoResponse GenerateCustomerDtoResponse(Customer customer);
        public Customer GenerateListAddressForCustomerAndReturnCustomer(CustomerDto customer);
        public void SaveChanges();
    }
}