using System.ComponentModel.DataAnnotations;
using CustomerManagement.Utils;

namespace CustomerManagement.DTO
{
    public class AddressDto
    {
        [Required(ErrorMessage = ResponseMessagesCustomers.ZipCodeIsRequired)]
        public string ZipCode { get; set; }
        [Required(ErrorMessage = ResponseMessagesCustomers.StreetIsRequired)]
        public string Street { get; set; }
        [Required(ErrorMessage = ResponseMessagesCustomers.NumberIsRequired)]
        public int Number { get; set; }
        [Required(ErrorMessage = ResponseMessagesCustomers.NeighborhoodIsRequired)]
        public string Neighborhood { get; set; }
        [Required(ErrorMessage = ResponseMessagesCustomers.AddressComplementIsRequired)]
        public string AddressComplement { get; set; }
        [Required(ErrorMessage = ResponseMessagesCustomers.CityIsRequired)]
        public string City { get; set; }
        [Required(ErrorMessage = ResponseMessagesCustomers.StateIsRequired)]
        public string State { get; set; }
        [Required(ErrorMessage = ResponseMessagesCustomers.CountryIsRequired)]
        public string Country { get; set; }
    }
}