using CustomerManagement.DTO;
using CustomerManagement.Models;
using CustomerManagement.Utils;

namespace CustomerManagement.Services
{
    public interface IOrderServices
    {
        public ServiceResult<Order> Add(OrderDtoRequest orderDtoRequest);
    }
}