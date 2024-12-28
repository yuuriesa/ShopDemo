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

            var orderReponse = _orderServices.GenerateOrderDtoResponse(order: result.Data);

            return CreatedAtAction(nameof(Add), new { OrderId = result.Data.OrderId} , value: orderReponse);
        }

        [HttpPost("batch")]
        public IActionResult AddBatchOrders([FromBody] IEnumerable<OrderDtoRequestBatch> listOrderDtoRequests)
        {
            var result = _orderServices.AddBatchOrders(listOrderDtoRequests: listOrderDtoRequests);

            return Ok();
        }
    }
}