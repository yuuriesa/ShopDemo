using System.Text.Json.Serialization;

namespace CustomerManagement.Models
{
    public class Order
    {
        public int OrderId { get; private set; }
        private int _number { get; set; }
        private DateTime _date { get; set; }
        private decimal _totalOrderValue;
        private int _customerId;
        public int Number => _number; //o pedido deve ter um número
        public DateTime Date => _date; //data
        public int CustomerId => _customerId;
        [JsonIgnore]
        public Customer Customer { get; set; } //qual cliente fez o pedido
        public ICollection<Item> Itens { get; private set; } = new List<Item>(); //itens do pedido
        public decimal TotalOrderValue => _totalOrderValue; //valor total do pedido

        public bool IsValid { get; private set; }

        //constructors
        //private contructor
        private Order(){}
        private Order
        (
            int orderId,
            int customerId,
            int number,
            DateTime date,
            List<Item> itens,
            decimal totalOrderValue
        )
        {
            OrderId = orderId;
            _customerId = customerId;
            _number = number;
            _date = date;
            Itens = itens;
            _totalOrderValue = totalOrderValue;
        }

        //public methods
        public static Order RegisterNew
        (
            int number,
            DateTime date,
            List<Item> itens
        )
        {
            var order = new Order();
            order.SetNumber(number: number);
            order.SetDate(date: date);
            order.SetTotalOrderValue(itens: itens);
            order.Validate();

            return order;
        }

        public static Order SetExistingInfo
        (
            int orderId,
            int customerId,
            int number,
            DateTime date,
            List<Item> itens,
            decimal totalOrderValue
        )
        {
            var order = new Order
            (
                orderId: orderId,
                customerId: customerId,
                number: number,
                date: date,
                itens: itens,
                totalOrderValue: totalOrderValue
            );
            order.Validate();

            return order;
        }

        // private methods
        private void SetNumber(int number)
        {
            if (number < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(number), "number must be greater than zero");
            }

            _number = number;
        }

        private void SetDate(DateTime date)
        {
            var dateNow = DateTime.UtcNow;
            if (date.ToUniversalTime().Date > dateNow.Date)
            {
                throw new ArgumentOutOfRangeException(nameof(date), "You cannot put the date with the day after today.");
            }

            _date = date;
        }

        private void SetTotalOrderValue(List<Item> itens)
        {
            var totalValue = from item in itens select item.TotalValue;
            _totalOrderValue = totalValue.Sum();
        }

        private void Validate()
        {
            var dateNow = DateTime.UtcNow;
            IsValid = _number > 0 && _date.ToUniversalTime().Date < dateNow.Date && _totalOrderValue > 0 && _customerId > 0 && Itens.Count > 0;
        }
    }
}