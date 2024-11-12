using CustomerManagement.DTO;
using CustomerManagement.Models;
using CustomerManagement.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CustomerManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private ICustomerRepository _repository;

        public CustomersController(ICustomerRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var allCustomers = _repository.GetAll();

            if (allCustomers.Count() == 0)
            {
                return NoContent();
            }

            return Ok(allCustomers);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var findCustomer = _repository.GetById(id);

            if (findCustomer == null)
            {
                return NotFound();
            }

            return Ok(findCustomer);
        }

        [HttpPost]
        public IActionResult Add([FromBody] CustomerDto customer)
        {
            var newCustomer = new Customer
            {
                Name = customer.Name,
                LastName = customer.LastName, 
                Email = customer.Email, 
                DateOfBirth = customer.DateOfBirth
             };

            _repository.Add(newCustomer);

            return CreatedAtAction(nameof(GetById), new {id = newCustomer.CustomerId}, customer);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Customer customer)
        {
            customer.CustomerId = id;
            _repository.Update(id, customer);

            
            return Ok(customer);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _repository.Delete(id);
            
            
            return NoContent();
        }
    }
}