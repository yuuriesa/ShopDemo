using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerManagement.Data;
using CustomerManagement.Models;

namespace CustomerManagement.Repository
{
    public class CustomerRepository : RepositoryBase<Customer>, ICustomerRepository
    {
        private readonly IApplicationDbContext _dbContext;

        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
            _dbContext = context;
        }

        public Customer GetByEmail(string email)
        {
            var findCustomerByEmail = _dbContext.Customers.FirstOrDefault(e => e.Email == email);

            if (findCustomerByEmail == null)
            {
                return null!;
            }

            return findCustomerByEmail;
        }
    }
}