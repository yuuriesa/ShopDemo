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
            var result = _services.Add(customer);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Message);
            }

            _services.SaveChanges();

            return CreatedAtAction(actionName: nameof(GetById), routeValues: new {id = result.Data.CustomerId}, value: result.Data);
        }

        [HttpPost("batch")]
        public IActionResult AddListCustomers([FromBody] IEnumerable<CustomerDto> customers)
        {
            if (customers.Count() == 0) return NoContent();

            var result = _services.AddRange(customers);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Message);
            }

            _services.SaveChanges();
                      
            var listCustomersForResponse = _services.GetListCustomersByEmail(customers);

            return Created("", listCustomersForResponse);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] CustomerDto customerDto)
        {
            var result = _services.Update(id, customerDto);
            
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Message);
            }

            _services.SaveChanges();
        
            return Ok(result.Data);
        }

        [HttpPatch("{id}")]
        public IActionResult UpdatePatch(int id, [FromBody] CustomerPatchDto customerPatchDto)
        {
            var result = _services.UpdatePatch(id, customerPatchDto);
            
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Message);
            }
            
            _services.SaveChanges();
        
            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _services.Delete(id);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Message);
            }

            _services.SaveChanges();
                       
            return NoContent();
        }
    }
}