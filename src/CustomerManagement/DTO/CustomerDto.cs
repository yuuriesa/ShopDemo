using System.ComponentModel.DataAnnotations;

namespace CustomerManagement.DTO
{
    public class CustomerDto
    {
        [Required(ErrorMessage = "FirstName is Required", AllowEmptyStrings = false)]
        [MaxLength(40, ErrorMessage = "Must have a maximum of 40 characters")]
        public string FirstName { get; set; }
        [MaxLength(40, ErrorMessage = "Must have a maximum of 40 characters")]
        public string? LastName { get; set; }
        [EmailAddress(ErrorMessage = "The Email field is not a valid e-mail address.")]
        [Required(ErrorMessage = "Email is Required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Date of Birth is Required")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        public ICollection<AddressDto> Addresses { get; set; }
    }
}