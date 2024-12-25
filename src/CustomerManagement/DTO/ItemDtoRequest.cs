using CustomerManagement.Models;

namespace CustomerManagement.DTO
{
    public class ItemDtoRequest
    {
        public ProductDtoRequest Product { get; set; } //cada item do pedido deve ter o produto
        public int QuantityOfItens { get; set; } //quantidade de itens
        public decimal UnitValue { get; set; } //valor unitário
    }
}