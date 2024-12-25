using System.Text.Json.Serialization;

namespace CustomerManagement.Models
{
    public class Item
    {
        public int ItemId { get; set; }
        public int QuantityOfItens { get; set; } //quantidade de itens
        public decimal UnitValue { get; set; } //valor unitário
        public decimal TotalValue { get; set; } //valor total do item
        public Product Product { get; set; } //cada item do pedido deve ter o produto
        public int OrderId { get; set; }
        [JsonIgnore]
        public Order Order { get; set; }
    }
}