using System.ComponentModel.DataAnnotations;

namespace CustomerManagement.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }
        [Required(ErrorMessage = "Name is Required", AllowEmptyStrings = false)]
        public string? Name { get; set; }
        [Required(ErrorMessage = "LastName is Required", AllowEmptyStrings = false)]
        public string? LastName { get; set; }
        [EmailAddress(ErrorMessage = "Please provide a valid email...")]
        [Required(ErrorMessage = "Email is Required")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Date of Birth is Required")]
        public DateTime DateOfBirth { get; set; }
    }
}