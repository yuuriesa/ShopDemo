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
            var allCustomers = _repository.GetAll(validFilter).Include(c => c.Addresses);
            return allCustomers;
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

            if (dateIsValid) return ServiceResult<Customer>.ErrorResult(ResponseMessagesCustomers.DateOfBirthError, 400);

            var findCustomerByEmail = GetByEmail(customer.Email);

            if (findCustomerByEmail != null) return ServiceResult<Customer>.ErrorResult(ResponseMessagesCustomers.EmailExistsError, 409);

            if (customer.Addresses.Count == 0) return ServiceResult<Customer>.ErrorResult(ResponseMessagesCustomers.MinimumRegisteredAddressError, 422);

            var checkIfTheCustomerHasARepeatingAddress = CheckIfTheCustomerHasARepeatingAddressInList(customer.Addresses);

            if (checkIfTheCustomerHasARepeatingAddress)
            {
                return ServiceResult<Customer>.ErrorResult(ResponseMessagesCustomers.DuplicateAddressExistsError, 422);
            }

            var newCustomer = GenerateListAddressForCustomerAndReturnCustomer(customer);

            _repository.Add(newCustomer);

            return ServiceResult<Customer>.SuccessResult(newCustomer, 201);
        }

        public ServiceResult<Customer> AddAddressInCustomer(int id, AddressDto addressDto)
        {
            var findCustomer = _repository.GetById(id);
            if (findCustomer == null) return ServiceResult<Customer>.ErrorResult(ResponseMessagesCustomers.CustomerNotFoundMessage, 404);

            var getAddressesByCustomerId = _addressRepository.GetAllAddressesByIdCustomer(id);

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

            var checkIfTheCustomerHasARepeatingAddress = CheckIfTheCustomerHasARepeatingAddressInDatabase(getAddressesByCustomerId, addressDto);

            if (checkIfTheCustomerHasARepeatingAddress.StatusCode == 422)
            {
                return ServiceResult<Customer>.ErrorResult(checkIfTheCustomerHasARepeatingAddress.Message, 422);
            }

            findCustomer.Addresses.Add(newAddress);

            _repository.Update(id, findCustomer);

            return ServiceResult<Customer>.SuccessResult(findCustomer, 201);
        }

        public ServiceResult<Customer> CheckIfTheCustomerHasARepeatingAddressInDatabase(IEnumerable<Address> addresses, AddressDto addressDto)
        {
            foreach (var address in addresses)
            {
                var addressExists = address.ZipCode == addressDto.ZipCode &&
                                    address.Street == addressDto.Street &&
                                    address.Number == addressDto.Number &&
                                    address.Neighborhood == addressDto.Neighborhood &&
                                    address.AddressComplement == addressDto.AddressComplement &&
                                    address.City == addressDto.City &&
                                    address.State == addressDto.State &&
                                    address.Country == addressDto.Country;
                if (addressExists)
                {
                    return ServiceResult<Customer>.ErrorResult(ResponseMessagesCustomers.AddressAlreadyBelongsToCustomerError, 422);
                }
            }

            return ServiceResult<Customer>.SuccessResult(null!, 200);
        }

        public bool CheckIfTheCustomerHasARepeatingAddressInList(IEnumerable<AddressDto> addresses)
        {
            List<AddressDto> listAddresses = new List<AddressDto>();

            foreach (var address in addresses)
            {
                var addressExists = listAddresses.Any(a => a.ZipCode == address.ZipCode &&
                                    a.Street == address.Street &&
                                    a.Number == address.Number &&
                                    a.Neighborhood == address.Neighborhood &&
                                    a.AddressComplement == address.AddressComplement &&
                                    a.City == address.City &&
                                    a.State == address.State &&
                                    a.Country == address.Country);
                if (addressExists)
                {
                    return true;
                }
                listAddresses.Add(address);
            }

            return false;
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
                return ServiceResult<IEnumerable<Customer>>.ErrorResult($"{ResponseMessagesCustomers.DuplicateEmailFoundError}: {string.Join(", ", duplicateEmails)}.", 400);
            }
            
            foreach (var customer in customers)
            {
                var dateIsValid = VerifyDateOfBirth(customer.DateOfBirth);

                if (dateIsValid) return ServiceResult<IEnumerable<Customer>>.ErrorResult(ResponseMessagesCustomers.DateOfBirthError, 400);

                var findCustomerByEmail = GetByEmail(customer.Email);

                if (findCustomerByEmail != null)
                {
                    return ServiceResult<IEnumerable<Customer>>.ErrorResult($"{ResponseMessagesCustomers.ThisEmailExistsError}: '{customer.Email}'", 409);
                }

                if (customer.Addresses.Count == 0)
                {
                    return ServiceResult<IEnumerable<Customer>>.ErrorResult(ResponseMessagesCustomers.MinimumRegisteredAddressError, 422);
                }

                var checkIfTheCustomerHasARepeatingAddress = CheckIfTheCustomerHasARepeatingAddressInList(customer.Addresses);

                if (checkIfTheCustomerHasARepeatingAddress)
                {
                    return ServiceResult<IEnumerable<Customer>>.ErrorResult(ResponseMessagesCustomers.DuplicateAddressExistsError, 422);
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

        public ServiceResult<BatchImportResponse> AddRange2(IEnumerable<CustomerDto> customers)
        {
            // BatchResponse
            var batchImportResponse = new BatchImportResponse();
            batchImportResponse.FailureErrorsMessages = new List<string>();
            batchImportResponse.Failure = new List<CustomerDto>();

            //Success
            List<Customer> listCustomersFinalResultSuccess = new List<Customer>();
            List<CustomerDto> listCustomersTemporaryResultSuccess = new List<CustomerDto>();

            //Failure
            List<CustomerDto> listCustomersFinalResultFailure = new List<CustomerDto>();

            var duplicateEmails = GetDuplicateEmails(customers: customers);
            if (duplicateEmails.Any()) batchImportResponse.FailureErrorsMessages.Add($"{ResponseMessagesCustomers.DuplicateEmailFoundError}: {string.Join(", ", duplicateEmails)}.");
           
            //Criar uma lista de customers apenas com os customers que tem email duplicado e os agrupar.
            var customersWithDuplicateEmail = customers.GroupBy(c => c.Email).Where(group => group.Count() > 1).SelectMany(group => group).ToList();
            batchImportResponse.Failure.AddRange(customersWithDuplicateEmail);

            //Criar uma lista de customers apenas com os customers que não tem email duplicado e os agrupar. 
            var customersWithNonDuplicateEmail = customers.GroupBy(c => c.Email).Where(group => group.Count() == 1).SelectMany(group => group).ToList();

            foreach (var customer in customersWithNonDuplicateEmail)
            {
                var dateIsValid = VerifyDateOfBirth(customer.DateOfBirth);
                var findCustomerByEmail = GetByEmail(customer.Email);
                var checkIfTheCustomerHasARepeatingAddress = CheckIfTheCustomerHasARepeatingAddressInList(customer.Addresses);

                if (!dateIsValid && findCustomerByEmail == null
                    && customer.Addresses.Count() > 0 && !checkIfTheCustomerHasARepeatingAddress
                   )
                {
                    listCustomersTemporaryResultSuccess.Add(customer);
                }

                if (dateIsValid) batchImportResponse.FailureErrorsMessages.Add(ResponseMessagesCustomers.DateOfBirthError);
                if (findCustomerByEmail != null) batchImportResponse.FailureErrorsMessages.Add($"{ResponseMessagesCustomers.ThisEmailExistsError}: '{customer.Email}'");
                if (customer.Addresses.Count == 0) batchImportResponse.FailureErrorsMessages.Add(ResponseMessagesCustomers.MinimumRegisteredAddressError);
                if (checkIfTheCustomerHasARepeatingAddress) batchImportResponse.FailureErrorsMessages.Add(ResponseMessagesCustomers.DuplicateAddressExistsError);

                if (dateIsValid || findCustomerByEmail != null || customer.Addresses.Count() == 0 || checkIfTheCustomerHasARepeatingAddress)
                        listCustomersFinalResultFailure.Add(customer);
            }

            foreach (var customer in listCustomersTemporaryResultSuccess)
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

                listCustomersFinalResultSuccess.Add(newCustomer);      
            }

            _repository.AddRange(listCustomersFinalResultSuccess);

            // Lista para gerar a Response dos customers
            var listCustomersForCustomerDtoResponse = new List<CustomerDtoResponse>();

            foreach (var customer in listCustomersFinalResultSuccess)
            {
                var customerDto = GenerateCustomerDtoResponse(customer);
                listCustomersForCustomerDtoResponse.Add(customerDto);
            }

            if (listCustomersForCustomerDtoResponse.Count > 0)
            {
                batchImportResponse.Success = listCustomersForCustomerDtoResponse;
            }

            batchImportResponse.Failure.AddRange(listCustomersFinalResultFailure);
            batchImportResponse.SuccessCount = listCustomersFinalResultSuccess.Count;
            batchImportResponse.FailureCount = listCustomersFinalResultFailure.Count;

            return ServiceResult<BatchImportResponse>.SuccessResult(batchImportResponse, 201);
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

            if (dateIsValid) return ServiceResult<CustomerDtoResponse>.ErrorResult(ResponseMessagesCustomers.DateOfBirthError, 400);

            var findCustomer = GetById(id);

            if (findCustomer == null) return ServiceResult<CustomerDtoResponse>.ErrorResult(ResponseMessagesCustomers.CustomerNotFoundMessage, 404);

            var findCustomerByEmail = GetByEmail(customerDto.Email);

            if (findCustomerByEmail != null && findCustomerByEmail.CustomerId != id)
                return ServiceResult<CustomerDtoResponse>.ErrorResult(ResponseMessagesCustomers.ThisEmailExistsError, 409);

            if (customerDto.Addresses.Count == 0) return ServiceResult<CustomerDtoResponse>.ErrorResult(ResponseMessagesCustomers.MinimumRegisteredAddressError, 422);

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
            if (findCustomer == null) return ServiceResult<Customer>.ErrorResult(ResponseMessagesCustomers.CustomerNotFoundMessage, 404);

            var findAddress = _addressRepository.GetById((int)addressId);
            if (findAddress == null) return ServiceResult<Customer>.ErrorResult(ResponseMessagesCustomers.AddressNotFoundMessage, 404);
            if (findAddress.CustomerId != findCustomer.CustomerId) return ServiceResult<Customer>.ErrorResult(ResponseMessagesCustomers.ResourceWasNotFound, 404);

            var getAddressesByCustomerId = _addressRepository.GetAllAddressesByIdCustomer(id);

            var checkIfTheCustomerHasARepeatingAddress = CheckIfTheCustomerHasARepeatingAddressInDatabase(getAddressesByCustomerId, addressDto);

            if (checkIfTheCustomerHasARepeatingAddress.StatusCode == 422)
            {
                return ServiceResult<Customer>.ErrorResult(checkIfTheCustomerHasARepeatingAddress.Message, 422);
            }

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

            if (findCustomer == null) return ServiceResult<Customer>.ErrorResult(ResponseMessagesCustomers.CustomerNotFoundMessage, 404);

            if (customerPatchDto.Email != null)
            {
                var findCustomerByEmail = GetByEmail(customerPatchDto.Email);

                if (findCustomerByEmail != null && findCustomerByEmail.CustomerId != id)
                    return ServiceResult<Customer>.ErrorResult(ResponseMessagesCustomers.ThisEmailExistsError, 409);

                if (findCustomer.Email != customerPatchDto.Email)
                    findCustomer.Email = customerPatchDto.Email;
            }
            if (customerPatchDto.DateOfBirth != null)
            {
                var dateIsValid = VerifyDateOfBirth((DateTime)customerPatchDto.DateOfBirth);

                if (dateIsValid) return ServiceResult<Customer>.ErrorResult(ResponseMessagesCustomers.DateOfBirthError, 400);
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
            if (findCustomer == null) return ServiceResult<Customer>.ErrorResult(ResponseMessagesCustomers.CustomerNotFoundMessage, 404);
        
            var findAddress = _addressRepository.GetById((int)addressId);

            if (findAddress == null) return ServiceResult<Customer>.ErrorResult(ResponseMessagesCustomers.AddressNotFoundMessage, 404);
            if (findAddress.CustomerId != findCustomer.CustomerId) return ServiceResult<Customer>.ErrorResult(ResponseMessagesCustomers.ResourceWasNotFound, 404);

            var addressForUpdate = CheckWhichPropertiesToChangeAddressUpdatePatch(findAddress, addressPatchDto);

            findAddress =  addressForUpdate;
            
            _addressRepository.Update(id, findAddress);

            return ServiceResult<Customer>.SuccessResult(findCustomer);
        }

        public ServiceResult<Customer> Delete(int id)
        {
            var findCustomer = GetById(id);

            if (findCustomer == null) return ServiceResult<Customer>.ErrorResult(ResponseMessagesCustomers.CustomerNotFoundMessage, 404);

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
            if (findCustomer == null) return ServiceResult<Customer>.ErrorResult(ResponseMessagesCustomers.CustomerNotFoundMessage, 404);

            var findAddress = _addressRepository.GetById(addressId);
            if (findAddress == null) return ServiceResult<Customer>.ErrorResult(ResponseMessagesCustomers.AddressNotFoundMessage, 404);

            var addressExistsInCustomer = findCustomer.Addresses.Contains(findAddress);
            if (!addressExistsInCustomer)
            {
                return ServiceResult<Customer>.ErrorResult(ResponseMessagesCustomers.ResourceWasNotFound, 404);
            }

            var allAddresses = _addressRepository.GetAllAddressesByIdCustomer(id);
            if (allAddresses.Count() == 1)
            {
                return ServiceResult<Customer>.ErrorResult(ResponseMessagesCustomers.ItsNotPossibleToDeleteTheLastAddress, 422);
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