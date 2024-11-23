using CustomerManagement.DTO;
using CustomerManagement.Models;
using CustomerManagement.Repository;
using CustomerManagement.Utils;

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

        public bool VerifyDateOfBirth(DateTime customerDateOfBirth)
        {
            var dateNow = DateTime.UtcNow;

            if (customerDateOfBirth.ToUniversalTime().Date > dateNow.Date)
            {
                return true;
            }

            return false;
        }

        public ServiceResult<Customer> Add(CustomerDto customer)
        {
            var dateIsValid = VerifyDateOfBirth(customer.DateOfBirth);

            if (dateIsValid) return ServiceResult<Customer>.ErrorResult("You cannot put the date with the day after today.", 400);

            var findCustomerByEmail = GetByEmail(customer.Email);

            if (findCustomerByEmail != null) return ServiceResult<Customer>.ErrorResult("This email exists", 409);

            var newCustomer = new Customer
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName, 
                Email = customer.Email, 
                DateOfBirth = DateOnly.FromDateTime(customer.DateOfBirth)
             };

            _repository.Add(newCustomer);

            return ServiceResult<Customer>.SuccessResult(newCustomer, 201);
        }

        public Customer GetByEmail(string email)
        {
            var findCustomerByEmail = _repository.GetByEmail(email);
            return findCustomerByEmail;
        }

        public void SaveChanges()
        {
            _repository.SaveChanges();
        }

        public void AddRange(IEnumerable<CustomerDto> customers)
        {
            List<Customer> listCustomers = new List<Customer>();

            foreach (var customer in customers)
            {
                var newCustomer = new Customer
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName, 
                    Email = customer.Email, 
                    DateOfBirth = DateOnly.FromDateTime(customer.DateOfBirth)
                };

                listCustomers.Add(newCustomer);      
            }

            _repository.AddRange(listCustomers);
        }

        public IEnumerable<Customer> GetListCustomersByEmail(IEnumerable<CustomerDto> customers)
        {
            List<Customer> listCustomersForResponse = new List<Customer>();
            foreach (var customer in customers)
            {
                listCustomersForResponse.Add(_repository.GetByEmail(customer.Email));
            }

            return listCustomersForResponse;
        }

        public ServiceResult<Customer> Update(int id, CustomerDto customerDto)
        {
            var dateIsValid = VerifyDateOfBirth(customerDto.DateOfBirth);

            if (dateIsValid) return ServiceResult<Customer>.ErrorResult("You cannot put the date with the day after today.", 400);

            var findCustomer = GetById(id);

            if (findCustomer == null) return ServiceResult<Customer>.ErrorResult("Customer not found.", 404);

            var findCustomerByEmail = GetByEmail(customerDto.Email);

            if (findCustomerByEmail != null && findCustomerByEmail.CustomerId != id)
                return ServiceResult<Customer>.ErrorResult("This Email exists.", 409);

            if (findCustomer.Email != customerDto.Email)
                    findCustomer.Email = customerDto.Email;

            findCustomer.CustomerId = id;
            findCustomer.FirstName = customerDto.FirstName;
            findCustomer.LastName = customerDto.LastName;
            findCustomer.DateOfBirth = DateOnly.FromDateTime(customerDto.DateOfBirth);
            
            _repository.Update(id, findCustomer);
            return ServiceResult<Customer>.SuccessResult(findCustomer);
        }

        public ServiceResult<Customer> UpdatePatch(int id, CustomerPatchDto customerPatchDto)
        {
            var findCustomer = GetById(id);

            if (findCustomer == null) return ServiceResult<Customer>.ErrorResult("Customer not found.", 404);

            if (customerPatchDto.Email != null)
            {
                var findCustomerByEmail = GetByEmail(customerPatchDto.Email);

                if (findCustomerByEmail != null && findCustomerByEmail.CustomerId != id)
                    return ServiceResult<Customer>.ErrorResult("This Email exists", 409);

                if (findCustomer.Email != customerPatchDto.Email)
                    findCustomer.Email = customerPatchDto.Email;
            }
            if (customerPatchDto.DateOfBirth != null)
            {
                var dateIsValid = VerifyDateOfBirth((DateTime)customerPatchDto.DateOfBirth);

                if (dateIsValid) return ServiceResult<Customer>.ErrorResult("You cannot put the date with the day after today.", 400);
                findCustomer.DateOfBirth = DateOnly.FromDateTime((DateTime)customerPatchDto.DateOfBirth);
            }
            if (customerPatchDto.FirstName != null)
            {
                findCustomer.FirstName = customerPatchDto.FirstName;
            }
            if (customerPatchDto.LastName != null)
            {
                findCustomer.LastName = customerPatchDto.LastName;
            }
            _repository.Update(id, findCustomer);

            return ServiceResult<Customer>.SuccessResult(findCustomer);
        }

        public ServiceResult<Customer> Delete(int id)
        {
            var findCustomer = GetById(id);

            if (findCustomer == null) return ServiceResult<Customer>.ErrorResult("Customer not found.", 404);

            _repository.Delete(id);

            return ServiceResult<Customer>.SuccessResult(findCustomer, 204);
        }
    }
}