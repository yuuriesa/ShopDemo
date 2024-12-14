using CustomerManagement.DTO;

namespace CustomerManagement.Models
{
    public class BatchImportResponse
    {
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<Customer>? Success { get; set; }
        public List<CustomerDto>? Failure { get; set; }
        public List<string> FailureErrorsMessages { get; set; }
    }
}