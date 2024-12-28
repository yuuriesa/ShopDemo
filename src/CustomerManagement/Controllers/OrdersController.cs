using CustomerManagement.Data;
using CustomerManagement.DTO;
using CustomerManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IOrderServices _orderServices;
        private readonly ICustomerServices _customerServices;
        private readonly IProductServices _productServices;

        public OrdersController
        (
            ApplicationDbContext dbContext,
            IOrderServices orderServices,
            ICustomerServices customerServices,
            IProductServices productServices
        )
        {
            _dbContext = dbContext;
            _orderServices = orderServices;
            _customerServices = customerServices;
            _productServices = productServices;
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
        public async Task<IActionResult> AddBatchOrders([FromBody] IEnumerable<OrderDtoRequestBatch> listOrderDtoRequests)
        {
            foreach (var order in listOrderDtoRequests)
            {
                var getCustomer = _customerServices.GetByEmail(order.Customer.Email);

                if (getCustomer is not null && order.Customer.Addresses is null)
                {
                    return StatusCode(400, "The past customer does not exist, to register a customer through the order add at least the address");
                }

                if (getCustomer is null)
                {
                    var transaction1 = await _dbContext.Database.BeginTransactionAsync();
                    //cenário 1 - adicionar o cliente se o cliente não existir, se não continua.
                    try
                    {
                        _orderServices.CreateCustomerForOrderIfCustomerDoesNotExist(listOrderDtoRequest: listOrderDtoRequests);

                        await _dbContext.SaveChangesAsync();
                        await transaction1.CommitAsync();
                    }
                    catch (Exception err)
                    {
                        await transaction1.RollbackAsync();
                        return StatusCode(500, err.Message);
                    }
                }

                foreach (var item in order.Itens)
                {
                    var getProduct = _productServices.GetByCode(item.Product.Code);
                    if (getProduct is null)
                    {
                        var transaction2 = await _dbContext.Database.BeginTransactionAsync();
                        //cenário 2 - adicionar o produto se o produto não existir
                        try
                        {
                            _orderServices.CreateProductForOrderIfProductDoesNotExist(listOrderDtoRequest: listOrderDtoRequests);

                            await _dbContext.SaveChangesAsync();
                            await transaction2.CommitAsync();
                        }
                        catch (Exception err)
                        {
                            await transaction2.RollbackAsync();
                            return StatusCode(500, err.Message);
                        }
                    }
                }
            }

            var transaction3 = await _dbContext.Database.BeginTransactionAsync();
            //cenário 3 - adicionar o pedido
            try
            {
                var result = _orderServices.AddBatchOrders(listOrderDtoRequests: listOrderDtoRequests);
                
                await _dbContext.SaveChangesAsync();
                await transaction3.CommitAsync();
            }
            catch (Exception err)
            {
                await transaction3.RollbackAsync();
                return StatusCode(500, err.Message);
            }


            return Ok();
        }
    }
}