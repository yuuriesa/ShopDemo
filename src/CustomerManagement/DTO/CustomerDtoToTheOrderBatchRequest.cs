using System.ComponentModel.DataAnnotations;
using CustomerManagement.Utils;

namespace CustomerManagement.DTO
{
    public class CustomerDtoToTheOrderBatchRequest
    {
        [Required(ErrorMessage = ResponseMessagesCustomers.FirstNameIsRequired, AllowEmptyStrings = false)]
        [MaxLength(40, ErrorMessage = ResponseMessagesCustomers.MaximumCharacters)]
        public string FirstName { get; set; }
        [MaxLength(40, ErrorMessage = ResponseMessagesCustomers.MaximumCharacters)]
        public string? LastName { get; set; }
        [EmailAddress(ErrorMessage = ResponseMessagesCustomers.EmailFieldIsNotAValid)]
        [Required(ErrorMessage = ResponseMessagesCustomers.EmailIsRequired)]
        public string Email { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        [Required(ErrorMessage = ResponseMessagesCustomers.AddressIsRequired)]
        public ICollection<AddressDto>? Addresses { get; set; }
    }
}