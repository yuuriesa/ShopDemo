using System.ComponentModel.DataAnnotations;

namespace CustomerManagement.Utils
{
    public static class ResponseMessagesCustomers
    {
        public const string CustomerNotFoundMessage = "Customer not found";
        public const string AddressNotFoundMessage = "Address not found";
        public const string ProductNotFoundMessage = "Product not found";
        public const string CustomerPaginationError = "The pagination parameters 'pageSize' and 'pageNumber' must be positive numbers. Check the values ​​provided.";
        public const string DateOfBirthError = "You cannot put the date with the day after today.";
        public const string EmailExistsError = "This email exists";
        public const string ProductWithThisCodeExists = "There is a product with this code";
        public const string AddressExistsError = "This address already exists";
        public const string DuplicateAddressExistsError = "Duplicate address found in input";
        public const string AddressAlreadyBelongsToCustomerError = "This address already belongs to the customer";
        public const string MinimumRegisteredAddressError = "The customer must have at least one registered address";
        public const string DuplicateEmailFoundError = "Duplicate email(s) found in input";
        public const string ResourceWasNotFound = "The requested resource was not found.";
        public const string ThisEmailExistsError = $"This email exists";
        public const string ItsNotPossibleToDeleteTheLastAddress = "It is not possible to delete the last address. The customer must have at least one registered address";
        public const string FieldsAreInvalidProduct = "Code or name fields are invalid, check the values ​​available.";
        public const string CodeIsRequired = "Code is Required";
        public const string NameIsRequired = "Name is Required";
        public const string FirstNameIsRequired = "FirstName is Required";
        public const string EmailIsRequired = "Email is Required";
        public const string AddressIsRequired = "At least one address is Required";
        public const string ZipCodeIsRequired = "ZipCode is Required";
        public const string StreetIsRequired = "Street is Required";
        public const string NumberIsRequired = "Number is Required";
        public const string NeighborhoodIsRequired = "Neighborhood is Required";
        public const string AddressComplementIsRequired = "AddressComplement is Required";
        public const string CityIsRequired = "City is Required";
        public const string StateIsRequired = "State is Required";
        public const string CountryIsRequired = "Country is Required";
        public const string MaximumCharacters = "Must have a maximum of 40 characters";
        public const string EmailFieldIsNotAValid = "The Email field is not a valid e-mail address.";
    }
}