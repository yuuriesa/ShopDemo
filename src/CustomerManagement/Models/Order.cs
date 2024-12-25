using System.Text.Json.Serialization;

namespace CustomerManagement.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int Number { get; set; } //o pedido deve ter um número
        public DateTime Date { get; set; } //data
        public int CustomerId { get; set; }
        [JsonIgnore]
        public Customer Customer { get; set; } //qual cliente fez o pedido
        public ICollection<Item> Itens { get; set; } = new List<Item>(); //itens do pedido
        public decimal TotalOrderValue { get; set; } //valor total do pedido
    }
}