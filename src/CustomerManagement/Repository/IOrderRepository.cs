using CustomerManagement.DTO;
using CustomerManagement.Models;
using CustomerManagement.Utils;

namespace CustomerManagement.Repository
{
    public interface IOrderRepository : IRepositoryBase<Order>
    {
        public bool GetOrderByNumber(string number);
    }
}