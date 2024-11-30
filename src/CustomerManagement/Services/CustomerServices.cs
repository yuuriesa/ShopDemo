using CustomerManagement.DTO;
using CustomerManagement.Models;
using CustomerManagement.Repository;
using CustomerManagement.Utils;
using CustomerManagement.Services;

namespace CustomerManagement.Services
{
    public class CustomerServices : ICustomerServices
    {
        private ICustomerRepository _repository;
        private IAddressRepository _addressRepository;

        public CustomerServices(ICustomerRepository repository, IAddressRepository addressRepository)
        {
            _repository = repository;
            _addressRepository = addressRepository;
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
            List<Customer> customersResponses = new List<Customer>();
            var allCustomers = _repository.GetAll(validFilter);
            foreach (var customer in allCustomers)
            {
                var findAddress = _addressRepository.GetAllAddressesByIdCustomer(customer.CustomerId);
                customer.Addresses = findAddress.ToList();
                customersResponses.Add(customer);
            };
            return customersResponses;
        }

        public Customer GetById(int id)
        {
            var findCustomer = _repository.GetById(id);
            if (findCustomer == null)
            {
                return null!;
            }
            var findAddress = _addressRepository.GetAllAddressesByIdCustomer(findCustomer.CustomerId);
            findCustomer.Addresses = findAddress.ToList();
            
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

            var newCustomer = GenerateListAddressForCustomerAndReturnCustomer(customer);

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

        public ServiceResult<IEnumerable<Customer>> AddRange(IEnumerable<CustomerDto> customers)
        {
            List<Customer> listCustomers = new List<Customer>();

            var duplicateEmails = GetDuplicateEmails(customers: customers);

            if (duplicateEmails.Any())
            {
                return ServiceResult<IEnumerable<Customer>>.ErrorResult($"Duplicate email(s) found in input: {string.Join(", ", duplicateEmails)}.", 400);
            }
            
            foreach (var customer in customers)
            {
                var dateIsValid = VerifyDateOfBirth(customer.DateOfBirth);

                if (dateIsValid) return ServiceResult<IEnumerable<Customer>>.ErrorResult("You cannot put the date with the day after today.", 400);

                var findCustomerByEmail = GetByEmail(customer.Email);

                if (findCustomerByEmail != null)
                {
                    return ServiceResult<IEnumerable<Customer>>.ErrorResult($"This email: '{customer.Email}' exists", 409);
                }       
            }


            foreach (var customer in customers)
            {
                var newCustomer = new Customer
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName, 
                    Email = customer.Email, 
                    DateOfBirth = DateOnly.FromDateTime(customer.DateOfBirth),
                    Addresses = new List<Address>()
                };

                foreach (var address in customer.Addresses)
                {
                    var newAddress = new Address
                    {
                        ZipCode = address.ZipCode,
                        Street = address.Street,
                        Number = address.Number,
                        Neighborhood = address.Neighborhood,
                        AddressComplement = address.AddressComplement,
                        City = address.City,
                        State = address.State,
                        Country = address.Country
                    };

                    newCustomer.Addresses.Add(newAddress);
                }

                listCustomers.Add(newCustomer);      
            }

            _repository.AddRange(listCustomers);

            return ServiceResult<IEnumerable<Customer>>.SuccessResult(listCustomers, 201);
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

        public ServiceResult<CustomerDtoResponse> Update(int id, CustomerDto customerDto)
        {
            var dateIsValid = VerifyDateOfBirth(customerDto.DateOfBirth);

            if (dateIsValid) return ServiceResult<CustomerDtoResponse>.ErrorResult("You cannot put the date with the day after today.", 400);

            var findCustomer = GetById(id);

            if (findCustomer == null) return ServiceResult<CustomerDtoResponse>.ErrorResult("Customer not found.", 404);

            var findCustomerByEmail = GetByEmail(customerDto.Email);

            if (findCustomerByEmail != null && findCustomerByEmail.CustomerId != id)
                return ServiceResult<CustomerDtoResponse>.ErrorResult("This Email exists.", 409);

            if (findCustomer.Email != customerDto.Email)
                    findCustomer.Email = customerDto.Email;

            findCustomer.CustomerId = id;
            findCustomer.FirstName = customerDto.FirstName;
            findCustomer.LastName = customerDto.LastName;
            findCustomer.DateOfBirth = DateOnly.FromDateTime(customerDto.DateOfBirth);
            findCustomer.Addresses = GenerateListAddressForCustomerAndReturnCustomer(customerDto).Addresses;
            
            _repository.Update(id, findCustomer);
            return ServiceResult<CustomerDtoResponse>.SuccessResult(GenerateCustomerDtoResponse(findCustomer), 200);
        }

        public ServiceResult<Customer> UpdatePatch(int id, CustomerPatchDto customerPatchDto, int? addressId)
        {
            var findCustomer = GetById(id);

            if (findCustomer == null) return ServiceResult<Customer>.ErrorResult("Customer not found.", 404);

            if (addressId != null && customerPatchDto.Address != null)
            {
                var findAddress = _addressRepository.GetById((int)addressId);

                if (findAddress == null) return ServiceResult<Customer>.ErrorResult("Address not found", 404);
                if (findAddress.CustomerId != findCustomer.CustomerId) return ServiceResult<Customer>.ErrorResult("The Address id does not belong to this client.", 409);

                if (customerPatchDto.Address.ZipCode != null)
                {
                    findAddress.ZipCode = customerPatchDto.Address.ZipCode;
                }
                if (customerPatchDto.Address.Street != null)
                {
                    findAddress.Street = customerPatchDto.Address.Street;
                }
                if (customerPatchDto.Address.Number != null)
                {
                    findAddress.Number = (int)customerPatchDto.Address.Number;
                }
                if (customerPatchDto.Address.Neighborhood != null)
                {
                    findAddress.Neighborhood = customerPatchDto.Address.Neighborhood;
                }
                if (customerPatchDto.Address.AddressComplement != null)
                {
                    findAddress.AddressComplement = customerPatchDto.Address.AddressComplement;
                }
                if (customerPatchDto.Address.City != null)
                {
                    findAddress.City = customerPatchDto.Address.City;
                }
                if (customerPatchDto.Address.State != null)
                {
                    findAddress.State = customerPatchDto.Address.State;
                }
                if (customerPatchDto.Address.Country != null)
                {
                    findAddress.Country = customerPatchDto.Address.Country;
                }
            }
            else if (addressId != null && customerPatchDto.Address == null)
            {
                return ServiceResult<Customer>.ErrorResult("Address cannot be null", 400);
            }
            else if (addressId == null && customerPatchDto.Address != null)
            {
                return ServiceResult<Customer>.ErrorResult("you cannot change the address without your addressId", 400);
            }

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

        public CustomerDtoResponse GenerateCustomerDtoResponse(Customer customer)
        {
            List<AddressDto> addresses = new List<AddressDto>();

            foreach (var address in customer.Addresses)
            {
                var newAddress = new AddressDto
                {
                    ZipCode = address.ZipCode,
                    Street = address.Street,
                    Number = address.Number,
                    Neighborhood = address.Neighborhood,
                    AddressComplement = address.AddressComplement,
                    City = address.City,
                    State = address.State,
                    Country = address.Country
                };

                addresses.Add(newAddress);
            }

            var newCustomerResponse = new CustomerDtoResponse
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName, 
                Email = customer.Email, 
                DateOfBirth = customer.DateOfBirth,
                Addresses = addresses
             };

             return newCustomerResponse;
        }

        public Customer GenerateListAddressForCustomerAndReturnCustomer(CustomerDto customer)
        {
            List<Address> addresses = new List<Address>();

            foreach (var address in customer.Addresses)
            {
                var newAddress = new Address
                {
                    ZipCode = address.ZipCode,
                    Street = address.Street,
                    Number = address.Number,
                    Neighborhood = address.Neighborhood,
                    AddressComplement = address.AddressComplement,
                    City = address.City,
                    State = address.State,
                    Country = address.Country
                };

                addresses.Add(newAddress);
            }

            var newCustomer = new Customer
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName, 
                Email = customer.Email, 
                DateOfBirth = DateOnly.FromDateTime(customer.DateOfBirth),
                Addresses = addresses
             };

            return newCustomer;
        }

        public IEnumerable<CustomerDtoResponse> GenerateListCustomerDtoResponses(List<Customer> customers)
        {
            List<CustomerDtoResponse> allCustomersDtoResponse = new List<CustomerDtoResponse>();

            foreach (var customer in customers)
            {
                var newCustomerResponse = GenerateCustomerDtoResponse(customer);
                allCustomersDtoResponse.Add(newCustomerResponse);
            }
            return allCustomersDtoResponse;
        }
    }
}