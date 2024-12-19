using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CustomerManagement.DTO;
using CustomerManagement.Utils;

namespace CustomerManagement.Models
{
    public class CustomerPatchDto
    {
        [MaxLength(40, ErrorMessage = ResponseMessagesCustomers.MaximumCharacters)]
        public string? FirstName { get; set; }
        
        [MaxLength(40, ErrorMessage = ResponseMessagesCustomers.MaximumCharacters)]
        public string? LastName { get; set; }

        [EmailAddress(ErrorMessage = ResponseMessagesCustomers.EmailFieldIsNotAValid)]
        public string? Email { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
    }

    public class CustomerPatchDtoResponse
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; set; }
    }
}