using CustomerManagement.Data;
using CustomerManagement.DTO;
using Microsoft.AspNetCore.Mvc;

namespace CustomerManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public OrdersController
        (
            ApplicationDbContext dbContext
        )
        {
            _dbContext = dbContext;
        }


        [HttpPost]
        public ActionResult Add([FromBody] OrderDtoRequest orderDtoRequest)
        {
            return Ok();
        }
    }
}