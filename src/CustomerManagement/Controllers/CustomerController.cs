using CustomerManagement.Models;
using CustomerManagement.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CustomerManagement.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerRepository _repository;

        public CustomerController(CustomerRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_repository.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            return Ok(_repository.GetById(id));
        }

        [HttpPost]
        public IActionResult Add([FromBody] Customer customer)
        {
            _repository.Add(customer);

            return CreatedAtAction(nameof(GetById), new {id = customer.CustomerId}, customer);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Customer customer)
        {
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