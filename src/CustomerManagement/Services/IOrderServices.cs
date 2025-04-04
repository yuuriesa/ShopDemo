using CustomerManagement.DTO;
using CustomerManagement.Models;
using CustomerManagement.Utils;

namespace CustomerManagement.Services
{
    public interface IOrderServices
    {
        public IEnumerable<Order> GetAll(PaginationFilter paginationFilter);
        public Order GetById(int id);
        public ServiceResult<Order> Add(OrderDtoRequest orderDtoRequest);
        public ServiceResult<IEnumerable<Order>> AddBatchOrders(IEnumerable<OrderDtoRequestBatch> listOrderDtoRequests);
        public void CreateCustomerForOrderIfCustomerDoesNotExist(OrderDtoRequestBatch orderDtoRequestBatch);
        public void CreateNewAddressForCustomerIfAddressDoesNotExist(IEnumerable<AddressDto> addresses, string email);
        public void CreateProductForOrderIfProductDoesNotExist(OrderDtoRequestBatch orderDtoRequestBatch);
        public OrderDtoResponse GenerateOrderDtoResponse(Order order);
        public List<string> GetDuplicateNumbersInOrders(IEnumerable<OrderDtoRequestBatch> listOrderDtoRequests);
        public void CheckForDuplicateEmail(IEnumerable<OrderDtoRequestBatch> listOrderDtoRequest);
    }
}