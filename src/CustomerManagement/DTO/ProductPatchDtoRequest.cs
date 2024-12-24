using System.ComponentModel.DataAnnotations;
using CustomerManagement.Utils;

namespace CustomerManagement.DTO
{
    public class ProductPatchDtoRequest
    {
        // Caso Precise mudar o Code Também.
        // [MaxLength(40, ErrorMessage = ResponseMessagesCustomers.MaximumCharacters)]
        // public string Code { get; set; }
        [MaxLength(40, ErrorMessage = ResponseMessagesCustomers.MaximumCharacters)]
        public string? Name { get; set; }
    }
}