using System.ComponentModel.DataAnnotations;

namespace CustomerManagement.DTO
{
    public class ProductDtoRequest
    {
        [Required(ErrorMessage = "Code is Required", AllowEmptyStrings = false)]
        [MaxLength(40, ErrorMessage = "Must have a maximum of 40 characters")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Name is Required", AllowEmptyStrings = false)]
        [MaxLength(40, ErrorMessage = "Must have a maximum of 40 characters")]
        public string Name { get; set; }
    }
}