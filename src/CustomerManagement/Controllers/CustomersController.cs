using System.ComponentModel.DataAnnotations;
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
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 0 || pageSize < 0)
            {
                return BadRequest();
            }

            var validFilter = new PaginationFilter(pageNumber: pageNumber, pageSize: pageSize);

            var allCustomers = await _repository.GetAll(validFilter);


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
            var dateNow = DateTime.UtcNow;

            if (customer.DateOfBirth.ToUniversalTime().Date > dateNow.Date)
            {
                return BadRequest("You cannot put the date with the day after today.");
            }

            var findCustomerByEmail = _repository.GetByEmail(customer.Email);

            if (findCustomerByEmail != null)
            {
                return Conflict("This email exists");
            }

            var newCustomer = new Customer
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName, 
                Email = customer.Email, 
                DateOfBirth = DateOnly.FromDateTime(customer.DateOfBirth)
             };

            _repository.Add(newCustomer);

            return CreatedAtAction(actionName: nameof(GetById), routeValues: new {id = newCustomer.CustomerId}, value: newCustomer);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] CustomerDto customerDto)
        {
            var updatedCustomer = new Customer {};

            updatedCustomer.CustomerId = id;
            updatedCustomer.FirstName = customerDto.FirstName;
            updatedCustomer.LastName = customerDto.LastName;
            updatedCustomer.Email = customerDto.Email;
            updatedCustomer.DateOfBirth = DateOnly.FromDateTime(customerDto.DateOfBirth);
            
            _repository.Update(id, updatedCustomer);

            
            return Ok(updatedCustomer);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _repository.Delete(id);
            
            
            return NoContent();
        }
    }
}