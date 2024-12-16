using CustomerManagement.DTO;

namespace CustomerManagement.Models
{
    public class BatchImportResponse
    {
        public int SuccessCustomersCount { get; set; }
        public int FailureCustomersCount { get; set; }
        public List<CustomerDtoResponse>? Success {get;set;}

        public List<CustomerDtoWithMessageError>? Failure { get; set; }
    }
}