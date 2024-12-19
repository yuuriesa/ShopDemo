using System.ComponentModel.DataAnnotations;

namespace CustomerManagement.DTO
{
    public class ProductDtoResponse
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}