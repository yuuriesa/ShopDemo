using CustomerManagement.Data;
using CustomerManagement.Repository;
using CustomerManagement.Utils;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Services
{
    public class OrderServices : IOrderServices
    {
        private IOrderRepository _orderRepository;
        private ApplicationDbContext _dbContext;

        public OrderServices(IOrderRepository orderRepository, ApplicationDbContext dbContext)
        {
            _orderRepository = orderRepository;
            _dbContext = dbContext;
        }
    }
}