using CustomerManagement.Data;
using CustomerManagement.DTO;
using CustomerManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Repository
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        private readonly IApplicationDbContext _dbContext;
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
            _dbContext = context;
        }

        public bool GetOrderByNumber(string number)
        {
            var numberExists = _dbContext.Orders.ToList().FirstOrDefault(o => o.Number == number);
            
            if (numberExists != null)
            {
                return true;
            }

            return false;
        }
    }
}