using CustomerManagement.Data;
using CustomerManagement.DTO;
using CustomerManagement.Models;
using CustomerManagement.Repository;
using CustomerManagement.Services;
using CustomerManagement.Utils;
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
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerServices _customerServices;
        private readonly IProductServices _productServices;

        public OrdersController
        (
            ApplicationDbContext dbContext,
            IOrderServices orderServices,
            IOrderRepository orderRepository,
            ICustomerServices customerServices,
            IProductServices productServices
        )
        {
            _dbContext = dbContext;
            _orderServices = orderServices;
            _orderRepository = orderRepository;
            _customerServices = customerServices;
            _productServices = productServices;
        }

        [HttpGet]
        public ActionResult GetAll(int pageNumber = 1, int pageSize = 10)
        {
            var paginationFilter = new PaginationFilter(pageNumber: pageNumber, pageSize: pageSize);

            var allOrders = _orderServices.GetAll(paginationFilter: paginationFilter);

            if (allOrders.Count() == 0)
            {
                return NoContent();
            }

            List<OrderDtoResponse> listOrdersResponse = new List<OrderDtoResponse>();

            foreach (var order in allOrders)
            {
                var orderReponse = _orderServices.GenerateOrderDtoResponse(order: order);

                listOrdersResponse.Add(item: orderReponse);
            }

            return Ok(listOrdersResponse.ToList());
        }


        [HttpGet("{id}")]
        public ActionResult GetById(int id)
        {
            var getOrderById = _orderServices.GetById(id: id);

            if (getOrderById == null)
            {
                return NotFound(ResponseMessagesCustomers.OrderNotFoundMessage);
            }

            var newOrderReponse = _orderServices.GenerateOrderDtoResponse(order: getOrderById);

            return Ok(newOrderReponse);
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

            return CreatedAtAction(nameof(GetById), new { id = result.Data.OrderId} , value: orderReponse);
        }

        [HttpPost("batch")]
        public async Task<IActionResult> AddBatchOrders([FromBody] IEnumerable<OrderDtoRequestBatch> listOrderDtoRequests)
        {
            //Se não tiver nenhum pedido na lista, retorna sem conteúdo
            if (listOrderDtoRequests.Count() == 0)
            {
                return NoContent();
            }

            //Verifica se tem números duplicados
            var duplicateNumbers = _orderServices.GetDuplicateNumbersInOrders(listOrderDtoRequests: listOrderDtoRequests);

            if (duplicateNumbers.Count > 0)
            {
                return StatusCode(value: "There are duplicate numbers in the orders", statusCode: 400);
            }

            //Verifica se número do pedido já existe e se a data é maior que a data de hoje para cada pedido
            foreach (var order in listOrderDtoRequests)
            {
                var numberExists = _orderRepository.GetOrderByNumber(number: order.Number);
            
                if (numberExists is true)
                {
                    return StatusCode(value: "This Order with this Number Exists", statusCode: 400);
                } 

                var verifyIfDateIsNotValid = DateVerify.CheckIfTheDateIsGreaterThanToday(datetime: order.Date);

                if (verifyIfDateIsNotValid)
                {
                    return StatusCode(value: ResponseMessagesCustomers.DateWithTheDayAfterToday, statusCode: 400);
                }
            }

            foreach (var order in listOrderDtoRequests)
            {
                var transaction1 = await _dbContext.Database.BeginTransactionAsync();

                try
                {
                    var getCustomer = _customerServices.GetByEmail(order.Customer.Email);

                    //Se o cliente não existir e não tiver endereço, retorna erro
                    if (getCustomer is null && order.Customer.Addresses is null)
                    {
                        return StatusCode(400, "The past customer does not exist, to register a customer through the order add at least the address");
                    }

                    //Se o cliente não existir e tiver endereço, cria o cliente
                    if (getCustomer is null && order.Customer.Addresses!.Count > 0)
                    {
                        _orderServices.CreateCustomerForOrderIfCustomerDoesNotExist(orderDtoRequestBatch: order);
                    }

                    //Se o cliente existir e for incluído um endereço que não existe, ele adiciona o enddereço novo!
                    if (getCustomer is not null && order.Customer.Addresses.Count > 0)
                    {
                        _orderServices.CreateNewAddressForCustomerIfAddressDoesNotExist(addresses: order.Customer.Addresses, email: order.Customer.Email);
                    }
          
                    //cenário 2 - adicionar o produto se o produto não existir
                    _orderServices.CreateProductForOrderIfProductDoesNotExist(orderDtoRequestBatch: order);


                    await _dbContext.SaveChangesAsync();
                    await transaction1.CommitAsync();
                }
                catch (Exception err)
                {
                    await transaction1.RollbackAsync();
                    return StatusCode(500, err.Message);
                }
            }

            var transaction2 = await _dbContext.Database.BeginTransactionAsync();
            //cenário 3 - adicionar o pedido
            try
            {
                var result = _orderServices.AddBatchOrders(listOrderDtoRequests: listOrderDtoRequests);
                
                if (!result.Success)
                {
                    return StatusCode(statusCode: result.StatusCode, value: result.Message);
                }

                await _dbContext.SaveChangesAsync();
                await transaction2.CommitAsync();

                List<OrderDtoResponse> listOrdersReponse = new List<OrderDtoResponse>();

                foreach (var order in result.Data)
                {
                    var newOrderReponse = _orderServices.GenerateOrderDtoResponse(order: order);

                    listOrdersReponse.Add(item: newOrderReponse);
                }

                return Ok(listOrdersReponse);
            }
            catch (Exception err)
            {
                await transaction2.RollbackAsync();
                return StatusCode(500, err.Message);
            }
        }
    }
}