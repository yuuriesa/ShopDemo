using CustomerManagement.DTO;
using CustomerManagement.Models;
using CustomerManagement.Repository;
using CustomerManagement.Utils;
using CustomerManagement.Services;
using Microsoft.EntityFrameworkCore;

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
            var allCustomers = _repository.GetAll(validFilter).Include(c => c.Addresses);
            foreach (var customer in allCustomers)
            {
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

            if (customer.Addresses.Count == 0) return ServiceResult<Customer>.ErrorResult("The customer must have at least one registered address", 422);

            var newCustomer = GenerateListAddressForCustomerAndReturnCustomer(customer);

            _repository.Add(newCustomer);

            return ServiceResult<Customer>.SuccessResult(newCustomer, 201);
        }

        public ServiceResult<Customer> AddAddressInCustomer(int id, AddressDto addressDto)
        {
            var findCustomer = _repository.GetById(id);
            if (findCustomer == null) return ServiceResult<Customer>.ErrorResult("Customer not found.", 404);

            var newAddress = new Address
            {
                ZipCode = addressDto.ZipCode,
                Street = addressDto.Street,
                Number = addressDto.Number,
                Neighborhood = addressDto.Neighborhood,
                AddressComplement = addressDto.AddressComplement,
                City = addressDto.City,
                State = addressDto.State,
                Country = addressDto.Country
            };

            findCustomer.Addresses.Add(newAddress);

            _repository.Update(id, findCustomer);

            return ServiceResult<Customer>.SuccessResult(findCustomer, 201);
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

                if (customer.Addresses.Count == 0)
                {
                    return ServiceResult<IEnumerable<Customer>>.ErrorResult("The customer must have at least one registered address", 422);
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

                var nonDuplicateAddresses = customer.Addresses
                    .GroupBy(a => new {
                        a.ZipCode, a.Street,
                        a.Number, a.Neighborhood,
                        a.AddressComplement, a.City,
                        a.State, a.Country})
                    .Select(group => group.First())
                    .ToArray();

                foreach (var address in nonDuplicateAddresses)
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

            if (customerDto.Addresses.Count == 0) return ServiceResult<CustomerDtoResponse>.ErrorResult("The customer must have at least one registered address", 422);

            if (findCustomer.Email != customerDto.Email)
                    findCustomer.Email = customerDto.Email;

            foreach (var address in findCustomer.Addresses)
            {
                _addressRepository.Delete(address.AddressId);
            }

            findCustomer.CustomerId = id;
            findCustomer.FirstName = customerDto.FirstName;
            findCustomer.LastName = customerDto.LastName;
            findCustomer.DateOfBirth = DateOnly.FromDateTime(customerDto.DateOfBirth);
            findCustomer.Addresses = GenerateListAddressForCustomerAndReturnCustomer(customerDto).Addresses;
            
            _repository.Update(id, findCustomer);
            return ServiceResult<CustomerDtoResponse>.SuccessResult(GenerateCustomerDtoResponse(findCustomer), 200);
        }

        public ServiceResult<Customer> UpdateAddress(int id, AddressDto addressDto, int addressId)
        {
            var findCustomer = GetById(id);
            if (findCustomer == null) return ServiceResult<Customer>.ErrorResult("Customer not found.", 404);

            var findAddress = _addressRepository.GetById((int)addressId);
            if (findAddress == null) return ServiceResult<Customer>.ErrorResult("Address not found", 404);
            if (findAddress.CustomerId != findCustomer.CustomerId) return ServiceResult<Customer>.ErrorResult("The requested resource was not found.", 404);

            findAddress.ZipCode = addressDto.ZipCode;
            findAddress.Street = addressDto.Street;
            findAddress.Number = addressDto.Number;
            findAddress.Neighborhood = addressDto.Neighborhood;
            findAddress.AddressComplement = addressDto.AddressComplement;
            findAddress.City = addressDto.City;
            findAddress.State = addressDto.State;
            findAddress.Country = addressDto.Country;

            
            _addressRepository.Update(id, findAddress);

            return ServiceResult<Customer>.SuccessResult(findCustomer);
        }

        public ServiceResult<Customer> UpdatePatchCustomer(int id, CustomerPatchDto customerPatchDto)
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

        public ServiceResult<Customer> UpdatePatchAddress(int id, AddressPatchDto addressPatchDto, int addressId)
        {
            var findCustomer = GetById(id);
            if (findCustomer == null) return ServiceResult<Customer>.ErrorResult("Customer not found.", 404);
        
            var findAddress = _addressRepository.GetById((int)addressId);

            if (findAddress == null) return ServiceResult<Customer>.ErrorResult("Address not found", 404);
            if (findAddress.CustomerId != findCustomer.CustomerId) return ServiceResult<Customer>.ErrorResult("The requested resource was not found.", 404);

            var addressForUpdate = CheckWhichPropertiesToChangeAddressUpdatePatch(findAddress, addressPatchDto);
            findAddress =  addressForUpdate;


            _addressRepository.Update(id, findAddress);

            return ServiceResult<Customer>.SuccessResult(findCustomer);
        }

        public ServiceResult<Customer> Delete(int id)
        {
            var findCustomer = GetById(id);

            if (findCustomer == null) return ServiceResult<Customer>.ErrorResult("Customer not found.", 404);

            foreach (var address in findCustomer.Addresses)
            {
                _addressRepository.Delete(address.AddressId);
            }

            _repository.Delete(id);

            return ServiceResult<Customer>.SuccessResult(findCustomer, 204);
        }

        public ServiceResult<Customer> DeleteAddress(int id, int addressId)
        {
            var findCustomer = _repository.GetById(id);
            if (findCustomer == null) return ServiceResult<Customer>.ErrorResult("Customer not found", 404);

            var findAddress = _addressRepository.GetById(addressId);
            if (findAddress == null) return ServiceResult<Customer>.ErrorResult("Address not found", 404);

            var addressExistsInCustomer = findCustomer.Addresses.Contains(findAddress);
            if (!addressExistsInCustomer)
            {
                return ServiceResult<Customer>.ErrorResult("The requested resource was not found.", 404);
            }

            var allAddresses = _addressRepository.GetAllAddressesByIdCustomer(id);
            if (allAddresses.Count() == 1)
            {
                return ServiceResult<Customer>.ErrorResult("It is not possible to delete the last address. The customer must have at least one registered address", 422);
            }

            _addressRepository.Delete(addressId);

            return ServiceResult<Customer>.SuccessResult(findCustomer);
        }

        public Address CheckWhichPropertiesToChangeAddressUpdatePatch(Address address, AddressPatchDto addressPatchDto)
        {
                if (addressPatchDto.ZipCode != null)
                {
                    address.ZipCode = addressPatchDto.ZipCode;
                }
                if (addressPatchDto.Street != null)
                {
                    address.Street = addressPatchDto.Street;
                }
                if (addressPatchDto.Number != null)
                {
                    address.Number = (int)addressPatchDto.Number;
                }
                if (addressPatchDto.Neighborhood != null)
                {
                    address.Neighborhood = addressPatchDto.Neighborhood;
                }
                if (addressPatchDto.AddressComplement != null)
                {
                    address.AddressComplement = addressPatchDto.AddressComplement;
                }
                if (addressPatchDto.City != null)
                {
                    address.City = addressPatchDto.City;
                }
                if (addressPatchDto.State != null)
                {
                    address.State = addressPatchDto.State;
                }
                if (addressPatchDto.Country != null)
                {
                    address.Country = addressPatchDto.Country;
                }

                return address;
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
                CustomerId = customer.CustomerId,
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
                    .GroupBy(a => new {
                        a.ZipCode, a.Street,
                        a.Number, a.Neighborhood,
                        a.AddressComplement, a.City,
                        a.State, a.Country})
                    .Select(group => group.First())
                    .ToArray()
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