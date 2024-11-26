using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CustomerManagement.DTO;

namespace CustomerManagement.Models
{
    public class CustomerPatchDto
    {
        [MaxLength(40, ErrorMessage = "Must have a maximum of 40 characters")]
        public string? FirstName { get; set; }
        
        [MaxLength(40, ErrorMessage = "Must have a maximum of 40 characters")]
        public string? LastName { get; set; }

        [EmailAddress(ErrorMessage = "The Email field is not a valid e-mail address.")]
        public string? Email { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        public ICollection<AddressDto> Addresses { get; set; }
    }
}