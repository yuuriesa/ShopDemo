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
            _orderServices.Add(orderDtoRequest: orderDtoRequest);
            return Ok();
        }
    }
}