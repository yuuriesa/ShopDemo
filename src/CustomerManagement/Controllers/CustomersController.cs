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
        public IActionResult GetAll(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 0 || pageSize < 0)
            {
                return BadRequest();
            }

            var validFilter = new PaginationFilter(pageNumber: pageNumber, pageSize: pageSize);

            var allCustomers = _repository.GetAll(validFilter);


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

        [HttpPost("add-list")]
        public IActionResult AddListCustomers([FromBody] IEnumerable<CustomerDto> customers)
        {
            var dateNow = DateTime.UtcNow;
            
            foreach (var customer in customers)
            {
                if (customer.DateOfBirth.ToUniversalTime().Date > dateNow)
                {
                    return BadRequest("You cannot put the date with the day after today.");
                }

                var findCustomerByEmail = _repository.GetByEmail(customer.Email);

                if (findCustomerByEmail != null)
                {
                    return Conflict($"This email: '{customer.Email}' exists");
                }       
            }

            List<Customer> listCustomersForResponse = new List<Customer>();

            foreach (var customer in customers)
            {
                var newCustomer = new Customer
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName, 
                    Email = customer.Email, 
                    DateOfBirth = DateOnly.FromDateTime(customer.DateOfBirth)
                };

                _repository.Add(newCustomer);      
                listCustomersForResponse.Add(_repository.GetByEmail(customer.Email));
            }

            return Created("", listCustomersForResponse);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] CustomerDto customerDto)
        {
            var findCustomer = _repository.GetById(id);

            if (findCustomer == null)
            {
                return NotFound();
            }

            var findCustomerByEmail = _repository.GetByEmail(customerDto.Email);

            if (findCustomerByEmail != null)
            {
                return Conflict("This Email exists");
            }

            findCustomer.CustomerId = id;
            findCustomer.FirstName = customerDto.FirstName;
            findCustomer.LastName = customerDto.LastName;
            findCustomer.Email = customerDto.Email;
            findCustomer.DateOfBirth = DateOnly.FromDateTime(customerDto.DateOfBirth);
            
            _repository.Update(id, findCustomer);

            
            return Ok(findCustomer);
        }

        [HttpPatch("{id}")]
        public IActionResult UpdatePatch(int id, [FromBody] CustomerPatchDto customerPatchDto)
        {
            var findCustomer = _repository.GetById(id);

            if (findCustomer == null)
            {
                return NotFound();
            }

            if (customerPatchDto.Email != null)
            {
                var findCustomerByEmail = _repository.GetByEmail(customerPatchDto.Email);

                if (findCustomerByEmail != null)
                {
                    return Conflict("This Email exists");
                }

                findCustomer.Email = customerPatchDto.Email;
            }
            if (customerPatchDto.FirstName != null)
            {
                findCustomer.FirstName = customerPatchDto.FirstName;
            }
            if (customerPatchDto.LastName != null)
            {
                findCustomer.LastName = customerPatchDto.LastName;
            }
            if (customerPatchDto.DateOfBirth != null)
            {
                findCustomer.DateOfBirth = DateOnly.FromDateTime((DateTime)customerPatchDto.DateOfBirth);
            }

            //findCustomer.CustomerId = id;
            
            _repository.Update(id, findCustomer);
        
            return Ok(findCustomer);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var findCustomer = _repository.GetById(id);

            if (findCustomer == null)
            {
                return NotFound();
            }

            _repository.Delete(id);
            
            
            return NoContent();
        }
    }
}