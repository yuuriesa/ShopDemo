using System.ComponentModel.DataAnnotations;

namespace CustomerManagement.DTO
{
    public class CustomerDto
    {
        [Required(ErrorMessage = "Name is Required", AllowEmptyStrings = false)]
        [MinLength(2, ErrorMessage = "Must Be at least 2 characters long")]
        [MaxLength(40, ErrorMessage = "Must have a maximum of 40 characters")]
        [RegularExpression(@"^[a-zA-Z]{2,40}$")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "LastName is Required", AllowEmptyStrings = false)]
        [MinLength(2, ErrorMessage = "Must Be at least 2 characters long")]
        [MaxLength(40, ErrorMessage = "Must have a maximum of 40 characters")]
        [RegularExpression(@"^[a-zA-Z]{2,40}$")]
        public string LastName { get; set; }
        [EmailAddress(ErrorMessage = "The Email field is not a valid e-mail address.")]
        [Required(ErrorMessage = "Email is Required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Date of Birth is Required")]
        public DateTime DateOfBirth { get; set; }
    }
}