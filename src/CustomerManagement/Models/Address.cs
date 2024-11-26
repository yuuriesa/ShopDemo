using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CustomerManagement.Models
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }
        [Required(ErrorMessage = "ZipCode is Required")]
        public string ZipCode { get; set; }
        [Required(ErrorMessage = "Street is Required")]
        public string Street { get; set; }
        [Required(ErrorMessage = "Number is Required")]
        public int Number { get; set; }
        [Required(ErrorMessage = "Neighborhood is Required")]
        public string Neighborhood { get; set; }
        [Required(ErrorMessage = "AddressComplement is Required")]
        public string AddressComplement { get; set; }
        [Required(ErrorMessage = "City is Required")]
        public string City { get; set; }
        [Required(ErrorMessage = "State is Required")]
        public string State { get; set; }
        [Required(ErrorMessage = "Country is Required")]
        public string Country { get; set; }
        [ForeignKey("CustomerId")]
        public int CustomerId { get; set; }
        [JsonIgnore]
        public Customer Customer { get; set; }
    }
}