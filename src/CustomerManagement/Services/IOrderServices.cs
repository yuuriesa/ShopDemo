using CustomerManagement.DTO;
using CustomerManagement.Models;
using CustomerManagement.Utils;

namespace CustomerManagement.Services
{
    public interface IOrderServices
    {
        public void Add(OrderDtoRequest orderDtoRequest);
    }
}