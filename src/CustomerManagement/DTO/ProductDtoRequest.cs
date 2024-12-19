using System.ComponentModel.DataAnnotations;
using CustomerManagement.Utils;

namespace CustomerManagement.DTO
{
    public class ProductDtoRequest
    {
        [Required(ErrorMessage = ResponseMessagesCustomers.CodeIsRequired, AllowEmptyStrings = false)]
        [MaxLength(40, ErrorMessage = ResponseMessagesCustomers.MaximumCharacters)]
        public string Code { get; set; }
        [Required(ErrorMessage = ResponseMessagesCustomers.NameIsRequired, AllowEmptyStrings = false)]
        [MaxLength(40, ErrorMessage = ResponseMessagesCustomers.MaximumCharacters)]
        public string Name { get; set; }
    }
}