namespace CustomerManagement.Utils
{
    public static class ResponseMessagesCustomers
    {
        public const string CustomerNotFoundMessage = "Customer not found";
        public const string AddressNotFoundMessage = "Address not found";
        public const string CustomerPaginationError = "The pagination parameters 'pageSize' and 'pageNumber' must be positive numbers. Check the values ​​provided.";
        public const string DateOfBirthError = "You cannot put the date with the day after today.";
        public const string EmailExistsError = "This email exists";
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
        public const string MaximumCharacters = "Must have a maximum of 40 characters";
    }
}