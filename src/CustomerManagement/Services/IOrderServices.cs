using CustomerManagement.DTO;
using CustomerManagement.Models;
using CustomerManagement.Utils;

namespace CustomerManagement.Services
{
    public interface IOrderServices
    {
        public ServiceResult<Order> Add(OrderDtoRequest orderDtoRequest);
        public ServiceResult<IEnumerable<Order>> AddBatchOrders(IEnumerable<OrderDtoRequestBatch> listOrderDtoRequests);
        public void CreateCustomerForOrderIfCustomerDoesNotExist(IEnumerable<OrderDtoRequestBatch> listOrderDtoRequest);
        public void CreateProductForOrderIfProductDoesNotExist(IEnumerable<OrderDtoRequestBatch> listOrderDtoRequest);
        public OrderDtoResponse GenerateOrderDtoResponse(Order order);
    }
}