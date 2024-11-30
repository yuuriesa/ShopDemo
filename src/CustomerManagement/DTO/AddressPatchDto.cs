using System.ComponentModel.DataAnnotations;

namespace CustomerManagement.DTO
{
    public class AddressPatchDto
    {
        public string? ZipCode { get; set; }
        public string? Street { get; set; }
        public int? Number { get; set; }
        public string? Neighborhood { get; set; }
        public string? AddressComplement { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
    }
}