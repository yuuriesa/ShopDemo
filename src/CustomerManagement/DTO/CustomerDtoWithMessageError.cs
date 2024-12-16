namespace CustomerManagement.DTO
{
    public class CustomerDtoWithMessageError
    {
        public CustomerDto Customer { get; set; }
        public List<string> FailureErrorMessages { get; set; }
    }
}