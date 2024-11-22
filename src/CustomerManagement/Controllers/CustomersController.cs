using System.ComponentModel.DataAnnotations;
using CustomerManagement.DTO;
using CustomerManagement.Models;
using CustomerManagement.Repository;
using CustomerManagement.Services;
using CustomerManagement.Utils;
using Microsoft.AspNetCore.Mvc;

namespace CustomerManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _repository;
        private readonly ICustomerServices _services;

        public CustomersController(ICustomerRepository repository, ICustomerServices services)
        {
            _repository = repository;
            _services = services;
        }

        [HttpGet]
        public IActionResult GetAll(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 0 || pageSize < 0) return BadRequest();

            var validFilter = new PaginationFilter(pageNumber: pageNumber, pageSize: pageSize);

            var allCustomers = _services.GetAll(validFilter);


            if (allCustomers.Count() == 0) return NoContent();

            return Ok(allCustomers);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var findCustomer = _services.GetById(id);

            if (findCustomer == null) return NotFound();

            return Ok(findCustomer);
        }

        [HttpPost]
        public IActionResult Add([FromBody] CustomerDto customer)
        {
            var dateIsValid = _services.VerifyDateOfBirth(customer.DateOfBirth);

            if (dateIsValid) return BadRequest("You cannot put the date with the day after today.");

            var findCustomerByEmail = _services.GetByEmail(customer.Email);

            if (findCustomerByEmail != null) return Conflict("This email exists");

            var newCustomer = _services.Add(customer);

            _services.SaveChanges();

            return CreatedAtAction(actionName: nameof(GetById), routeValues: new {id = newCustomer.CustomerId}, value: newCustomer);
        }

        [HttpPost("batch")]
        public IActionResult AddListCustomers([FromBody] IEnumerable<CustomerDto> customers)
        {
            if (customers.Count() == 0) return NoContent();

            List<Customer> listCustomersForResponse = new List<Customer>();

            var duplicateEmails = _services.GetDuplicateEmails(customers: customers);

            if (duplicateEmails.Any())
            {
                return BadRequest($"Duplicate email(s) found in input: {string.Join(", ", duplicateEmails)}.");
            }
            
            foreach (var customer in customers)
            {
                var dateIsValid = _services.VerifyDateOfBirth(customer.DateOfBirth);

                if (dateIsValid) return BadRequest("You cannot put the date with the day after today.");

                var findCustomerByEmail = _repository.GetByEmail(customer.Email);

                if (findCustomerByEmail != null)
                {
                    return Conflict($"This email: '{customer.Email}' exists");
                }       
            }

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
            }
            

            
            _repository.SaveChanges();
            
            foreach (var customer in customers)
            {
                listCustomersForResponse.Add(_repository.GetByEmail(customer.Email));
            }

            return Created("", listCustomersForResponse);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] CustomerDto customerDto)
        {
            var dateIsValid = new CustomerValidator().VerifyDateOfBirth(customerDto.DateOfBirth);

            if (dateIsValid) return BadRequest("You cannot put the date with the day after today.");

            var findCustomer = _repository.GetById(id);

            if (findCustomer == null) return NotFound();

            var findCustomerByEmail = _repository.GetByEmail(customerDto.Email);

            if (findCustomerByEmail != null && findCustomerByEmail.CustomerId != id)
                return Conflict("This Email exists");

            if (findCustomer.Email != customerDto.Email)
                    findCustomer.Email = customerDto.Email;

            findCustomer.CustomerId = id;
            findCustomer.FirstName = customerDto.FirstName;
            findCustomer.LastName = customerDto.LastName;
            //findCustomer.Email = customerDto.Email;
            findCustomer.DateOfBirth = DateOnly.FromDateTime(customerDto.DateOfBirth);
            
            _repository.Update(id, findCustomer);
            _repository.SaveChanges();

            
            return Ok(findCustomer);
        }

        [HttpPatch("{id}")]
        public IActionResult UpdatePatch(int id, [FromBody] CustomerPatchDto customerPatchDto)
        {
            var findCustomer = _repository.GetById(id);

            if (findCustomer == null) return NotFound();

            if (customerPatchDto.Email != null)
            {
                var findCustomerByEmail = _repository.GetByEmail(customerPatchDto.Email);

                if (findCustomerByEmail != null && findCustomerByEmail.CustomerId != id)
                    return Conflict("This Email exists");

                if (findCustomer.Email != customerPatchDto.Email)
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
                var dateIsValid = new CustomerValidator().VerifyDateOfBirth((DateTime)customerPatchDto.DateOfBirth);

                if (dateIsValid) return BadRequest("You cannot put the date with the day after today.");
                findCustomer.DateOfBirth = DateOnly.FromDateTime((DateTime)customerPatchDto.DateOfBirth);
            }

            _repository.Update(id, findCustomer);
            _repository.SaveChanges();
        
            return Ok(findCustomer);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var findCustomer = _repository.GetById(id);

            if (findCustomer == null) return NotFound();

            _repository.Delete(id);
            _repository.SaveChanges();
            
            
            return NoContent();
        }
    }
}