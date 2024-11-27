using System.ComponentModel.DataAnnotations;

namespace CustomerManagement.DTO
{
    public class CustomerDtoResponse
    {
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; }
        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; set; }
        public ICollection<AddressDto> Addresses { get; set; }
    }
}