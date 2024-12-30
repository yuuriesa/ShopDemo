using Azure;
using CustomerManagement.Data;
using CustomerManagement.DTO;
using CustomerManagement.Models;
using CustomerManagement.Repository;
using CustomerManagement.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CustomerManagement.Services
{
    public class OrderServices : IOrderServices
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ApplicationDbContext _dbContext;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerServices _customerServices;
        private readonly IProductRepository _productRepository;
        private readonly IProductServices _productServices;
        public OrderServices
        (
            IOrderRepository orderRepository,
            ApplicationDbContext dbContext,
            ICustomerRepository customerRepository,
            ICustomerServices customerServices,
            IProductRepository productRepository,
            IProductServices productServices
        )
        {
            _orderRepository = orderRepository;
            _dbContext = dbContext;
            _customerRepository = customerRepository;
            _customerServices = customerServices;
            _productRepository = productRepository;
            _productServices = productServices;
        }

        public ServiceResult<Order> Add(OrderDtoRequest orderDtoRequest)
        {
            var numberExists = _orderRepository.GetOrderByNumber(number: orderDtoRequest.Number);
            
            if (numberExists is true)
            {
                return ServiceResult<Order>.ErrorResult(message: "This Order with this Number Exists", statusCode: 400);
            } 

            var verifyIfDateIsNotValid = DateVerify.CheckIfTheDateIsGreaterThanToday(datetime: orderDtoRequest.Date);

            if (verifyIfDateIsNotValid)
            {
                return ServiceResult<Order>.ErrorResult(message: ResponseMessagesCustomers.DateWithTheDayAfterToday, statusCode: 400);
            }

            List<Item> listItens = new List<Item>();

            var customer = _customerRepository.GetByEmail(email: orderDtoRequest.Customer.Email);

            if (customer == null)
            {
                return ServiceResult<Order>.ErrorResult(message: ResponseMessagesCustomers.CustomerNotFoundMessage, statusCode: 404);
            }

            if
            (
                customer.FirstName != orderDtoRequest.Customer.FirstName ||
                customer.LastName != orderDtoRequest.Customer.LastName ||
                customer.DateOfBirth != DateOnly.FromDateTime(orderDtoRequest.Customer.DateOfBirth)
            )
            {
                return ServiceResult<Order>.ErrorResult(message: "This Customer has an invalid fields", statusCode: 400);
            }

            foreach (var item in orderDtoRequest.Itens)
            {
                var findProduct = _productServices.GetByCode(code: item.Product.Code);

                if (findProduct == null)
                {
                    return ServiceResult<Order>.ErrorResult(message: $"{ResponseMessagesCustomers.ProductNotFoundMessage}. Code: {item.Product.Code}", statusCode: 404);
                }

                if (findProduct.Name != item.Product.Name)
                {
                    return ServiceResult<Order>.ErrorResult(message: $"This product with this code: {item.Product.Code} has an invalid name", statusCode: 400);
                }

                var product = Product.SetExistingInfo(id: findProduct!.Id, code: item.Product.Code, name: item.Product.Name);

                if (!product.IsValid)
                {
                    return ServiceResult<Order>.ErrorResult(ResponseMessagesCustomers.FieldsAreInvalidProduct, 400);
                }

                var newItem = Item.RegisterNew
                (
                    product: product,
                    unitValue: item.UnitValue,
                    quantityOfItens: item.QuantityOfItens
                );

                if (!newItem.IsValid)
                {
                    return ServiceResult<Order>.ErrorResult("fields in item are invalid", 400);
                }

                listItens.Add(newItem);
            }


            var order = Order.RegisterNew
            (
                number: orderDtoRequest.Number,
                date: orderDtoRequest.Date,
                customerId: customer.CustomerId,
                itens: listItens
            );

            if (!order.IsValid)
            {
                return ServiceResult<Order>.ErrorResult("fields in order are invalid", 400);
            }

            _orderRepository.Add(order);

            return ServiceResult<Order>.SuccessResult(order, 201);
        }

        public ServiceResult<IEnumerable<Order>> AddBatchOrders(IEnumerable<OrderDtoRequestBatch> listOrderDtoRequest)
        {
            List<Order> listOrders = new List<Order>();

            var listCustomerDto = from customer in listOrderDtoRequest
                                select new CustomerDto()
                                {
                                    FirstName = customer.Customer.FirstName,
                                    LastName = customer.Customer.LastName,
                                    Email = customer.Customer.Email,
                                    DateOfBirth = customer.Customer.DateOfBirth,
                                    Addresses = customer.Customer.Addresses!
                                };
                                
            var duplicateEmails = _customerServices.GetDuplicateEmails(listCustomerDto);

            if (duplicateEmails.Count > 0)
            {
                throw new Exception(ResponseMessagesCustomers.DuplicateEmailFoundError);
            }

            foreach (var order in listOrderDtoRequest)
            {
                var numberExists = _orderRepository.GetOrderByNumber(number: order.Number);
            
                if (numberExists is true)
                {
                    return ServiceResult<IEnumerable<Order>>.ErrorResult(message: "This Order with this Number Exists", statusCode: 400);
                } 

                var verifyIfDateIsNotValid = DateVerify.CheckIfTheDateIsGreaterThanToday(datetime: order.Date);

                if (verifyIfDateIsNotValid)
                {
                    return ServiceResult<IEnumerable<Order>>.ErrorResult(message: ResponseMessagesCustomers.DateWithTheDayAfterToday, statusCode: 400);
                }

                var customer = _customerServices.GetByEmail(order.Customer.Email);

                List<Item> listItens = new List<Item>();

                foreach (var item in order.Itens)
                {
                    var getProduct = _productServices.GetByCode(item.Product.Code);

                    if (getProduct.Name != item.Product.Name)
                    {
                        return ServiceResult<IEnumerable<Order>>.ErrorResult(message: $"This product with this code: {item.Product.Code} has an invalid name", statusCode: 400);
                    }

                    var setProduct = Product.SetExistingInfo(id: getProduct.Id, code: item.Product.Code, name: item.Product.Name);

                    if (!setProduct.IsValid)
                    {
                        return ServiceResult<IEnumerable<Order>>.ErrorResult("Product is not valid", 400);
                    }

                    var newItem = Item.RegisterNew(product: setProduct, unitValue: item.UnitValue, quantityOfItens: item.QuantityOfItens);

                    if (!newItem.IsValid)
                    {
                        return ServiceResult<IEnumerable<Order>>.ErrorResult("fields in item are invalid", 400);
                    }

                    listItens.Add(newItem);
                }

                var newOrder = Order.RegisterNew
                (
                    number: order.Number,
                    date: order.Date,
                    customerId: customer.CustomerId,
                    itens: listItens
                );

                _orderRepository.Add(entity: newOrder);
                listOrders.Add(newOrder);
            }

            return ServiceResult<IEnumerable<Order>>.SuccessResult(listOrders);
        }

        public void CreateCustomerForOrderIfCustomerDoesNotExist(IEnumerable<OrderDtoRequestBatch> listOrderDtoRequest)
        {
            var listCustomerDto = from customer in listOrderDtoRequest
                                select new CustomerDto()
                                {
                                    FirstName = customer.Customer.FirstName,
                                    LastName = customer.Customer.LastName,
                                    Email = customer.Customer.Email,
                                    DateOfBirth = customer.Customer.DateOfBirth,
                                    Addresses = customer.Customer.Addresses!
                                };
                                
            var duplicateEmails = _customerServices.GetDuplicateEmails(listCustomerDto);

            if (duplicateEmails.Count > 0)
            {
                throw new Exception(ResponseMessagesCustomers.DuplicateEmailFoundError);
            }

            foreach (var order in listOrderDtoRequest)
            {
                var customerDto = new CustomerDto()
                {
                    FirstName = order.Customer.FirstName,
                    LastName = order.Customer.LastName,
                    Email = order.Customer.Email,
                    DateOfBirth = order.Customer.DateOfBirth,
                    Addresses = order.Customer.Addresses!
                };

                // passo 2 - 
                 var dateIsValid = DateVerify.CheckIfTheDateIsGreaterThanToday(datetime: customerDto.DateOfBirth);

                if (dateIsValid) throw new Exception(ResponseMessagesCustomers.DateOfBirthError);

                var findCustomerByEmail = _customerServices.GetByEmail(customerDto.Email);

                if (findCustomerByEmail is not null)
                {
                    continue;
                }

                if (customerDto.Addresses.Count == 0 && findCustomerByEmail is null) throw new Exception(ResponseMessagesCustomers.MinimumRegisteredAddressError);

                var checkIfTheCustomerHasARepeatingAddress = _customerServices.CheckIfTheCustomerHasARepeatingAddressInList(customerDto.Addresses);

                if (checkIfTheCustomerHasARepeatingAddress)
                {
                    throw new Exception(ResponseMessagesCustomers.DuplicateAddressExistsError);
                }

                var newCustomer = _customerServices.GenerateListAddressForCustomerAndReturnCustomer(customer: customerDto);

                _customerRepository.Add(entity: newCustomer);
            }
        }

        public void CreateProductForOrderIfProductDoesNotExist(IEnumerable<OrderDtoRequestBatch> listOrderDtoRequest)
        {
            var codeAdded = new List<string>();

            foreach (var order in listOrderDtoRequest)
            {
                foreach (var item in order.Itens)
                    {
                        var existingProduct = _productRepository.GetByCode(item.Product.Code);
                        if (existingProduct is not null || codeAdded.Contains(item.Product.Code))
                        {
                            continue;
                        }

                        var newProduct = Product.RegisterNew(code: item.Product.Code, name: item.Product.Name);

                        if (!newProduct.IsValid)
                        {
                            throw new Exception("Product is not valid");
                        }

                        if (!codeAdded.Contains(newProduct.Code))
                        {
                            _productRepository.Add(entity: newProduct);
                            codeAdded.Add(newProduct.Code);
                        }
                    }
            }
        }
    
        public OrderDtoResponse GenerateOrderDtoResponse(Order order)
        {
            var listItens = new List<ItemDtoResponse>();

            var newOrderDtoReponse = new OrderDtoResponse();

            newOrderDtoReponse.Number = order.Number;
            newOrderDtoReponse.Date = order.Date;
            newOrderDtoReponse.CustomerId = order.CustomerId;
            newOrderDtoReponse.TotalOrderValue = order.TotalOrderValue;

            foreach (var item in order.Itens)
            {
                var getProduct = _productServices.GetByCode(item.Code);

                var newItemDtoResponse = new ItemDtoResponse()
                {
                    Product = getProduct,
                    QuantityOfItens = item.QuantityOfItens,
                    UnitValue = item.UnitValue
                };

                listItens.Add(newItemDtoResponse);
            }

            newOrderDtoReponse.Itens = listItens;

            return newOrderDtoReponse;

        }

        public List<int> GetDuplicateNumbersInOrders(IEnumerable<OrderDtoRequestBatch> listOrderDtoRequests)
        {
            return listOrderDtoRequests
            .GroupBy(c => c.Number)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();
        }

        public List<string> GetDuplicateCodesInOrders(IEnumerable<OrderDtoRequestBatch> listOrderDtoRequests)
        {
            return listOrderDtoRequests
                .SelectMany(c => c.Itens)
                .GroupBy(c => c.Product.Code)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key).ToList();
        }

    }
}