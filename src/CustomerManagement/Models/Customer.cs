using System.ComponentModel.DataAnnotations;

namespace CustomerManagement.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }
        [Required(ErrorMessage = "FirstName is Required", AllowEmptyStrings = false)]
        [MaxLength(40, ErrorMessage = "Must have a maximum of 40 characters")]
        public string FirstName { get; set; }
        [MaxLength(40, ErrorMessage = "Must have a maximum of 40 characters")]
        public string? LastName { get; set; }
        [EmailAddress(ErrorMessage = "The Email field is not a valid e-mail address.")]
        [Required(ErrorMessage = "Email is Required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Date of Birth is Required")]
        public DateOnly DateOfBirth { get; set; }
        public ICollection<Address> Addresses { get; set; }
    }
}