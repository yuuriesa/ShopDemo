using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CustomerManagement.Models
{
    public class Address
    {
        public int AddressId { get; set; }
        public string ZipCode { get; set; }
        public string Street { get; set; }
        public int Number { get; set; }
        public string Neighborhood { get; set; }
        public string AddressComplement { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public int CustomerId { get; set; }
        [JsonIgnore]
        public Customer Customer { get; set; }
    }
}