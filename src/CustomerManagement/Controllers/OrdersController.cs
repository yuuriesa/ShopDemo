using CustomerManagement.Data;
using CustomerManagement.DTO;
using CustomerManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace CustomerManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IOrderServices _orderServices;

        public OrdersController
        (
            ApplicationDbContext dbContext,
            IOrderServices orderServices
        )
        {
            _dbContext = dbContext;
            _orderServices = orderServices;
        }


        [HttpPost]
        public ActionResult Add([FromBody] OrderDtoRequest orderDtoRequest)
        {
            var result = _orderServices.Add(orderDtoRequest: orderDtoRequest);

            if (!result.Success)
            {
                return StatusCode(statusCode: result.StatusCode, value: result.Message);
            }

            _dbContext.SaveChanges();

            return CreatedAtAction(nameof(Add), new { id = result.Data.OrderId} , value: result.Data);
        }
    }
}