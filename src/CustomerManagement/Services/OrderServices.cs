using CustomerManagement.Data;
using CustomerManagement.DTO;
using CustomerManagement.Models;
using CustomerManagement.Repository;
using CustomerManagement.Utils;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Services
{
    public class OrderServices : IOrderServices
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ApplicationDbContext _dbContext;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductServices _productServices;
        public OrderServices
        (
            IOrderRepository orderRepository,
            ApplicationDbContext dbContext,
            ICustomerRepository customerRepository,
            IProductRepository productRepository,
            IProductServices productServices
        )
        {
            _orderRepository = orderRepository;
            _dbContext = dbContext;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _productServices = productServices;
        }

        public void Add(OrderDtoRequest orderDtoRequest)
        {
            List<Item> listItens = new List<Item>();
            var customer = _customerRepository.GetById(orderDtoRequest.CustomerId);

            if (customer == null)
            {
                throw new Exception("Customer not found");
            }

            foreach (var item in orderDtoRequest.Itens)
            {
                var product = Product.RegisterNew(code: item.Product.Code, name: item.Product.Name);

                if (!product.IsValid)
                {
                    throw new Exception("Product is not valid");
                }

                _productRepository.Add(product);

                var newItem = Item.RegisterNew
                (
                    product: product,
                    unitValue: item.UnitValue,
                    quantityOfItens: item.QuantityOfItens
                );

                listItens.Add(newItem);
            }


            var order = Order.RegisterNew
            (
                number: orderDtoRequest.Number,
                date: orderDtoRequest.Date,
                customerId: orderDtoRequest.CustomerId,
                itens: listItens
            );
            _orderRepository.Add(order);
            _dbContext.SaveChanges();
        }
    }
}