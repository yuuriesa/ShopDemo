using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using CustomerManagement.Models;

namespace CustomerManagement.DTO
{
    public class OrderDtoRequestBatch
    {
        public int Number { get; set; } //o pedido deve ter um número
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } //data
        public CustomerDtoToTheOrderBatchRequest Customer { get; set; } //qual cliente fez o pedido
        public List<ItemDtoRequest> Itens { get; set; } //itens do pedido
    }
}