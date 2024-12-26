using System.Text.Json.Serialization;

namespace CustomerManagement.Models
{
    public class Item
    {
        public int ItemId { get; private set; }
        private int _quantityOfItens { get; set; }
        private decimal _unitValue { get; set; }
        //private int _orderId { get; set; }

        public int QuantityOfItens => _quantityOfItens; //quantidade de itens
        public decimal UnitValue => _unitValue; //valor unitário
        public decimal TotalValue { get; private set; } //valor total do item
        public Product Product { get; private set; } //cada item do pedido deve ter o produto
        public int OrderId { get; private set; } //cada item do pedido deve ter o id do pedido
        [JsonIgnore]
        public Order Order { get; set; }
        public string Code { get; private set; }

        public bool IsValid { get; private set; }

        //constructors
        //private contructor
        private Item(){}
        private Item
        (
            int itemId,
            Product product,
            int orderId,
            int quantityOfItens,
            decimal unitValue,
            string code
        )
        {
            ItemId = itemId;
            Product = product;
            OrderId = orderId;
            _quantityOfItens = quantityOfItens;
            _unitValue = unitValue;
            Code = code;
        }

        //public methods
        public static Item RegisterNew
        (
            Product product,
            decimal unitValue,
            int quantityOfItens
        )
        {
            var item = new Item();
            item.SetProduct(product: product);
            item.SetUnitValue(unitValue: unitValue);
            item.SetQuantityOfItens(quantityOfItens: quantityOfItens);
            item.SetTotalValue();
            item.Validate();

            return item;
        }

        public static Item SetExistingInfo(int itemId, Product product, int orderId, int quantityOfItens, decimal unitValue)
        {
            var item = new Item(itemId: itemId, product: product, orderId: orderId, quantityOfItens: quantityOfItens, unitValue: unitValue, code: product.Code);
            item.Validate();

            return item;
        }

        // private methods
        private void SetUnitValue(decimal unitValue)
        {
            if (unitValue <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(unitValue), "The unit value must be greater than zero.");
            }

            _unitValue = unitValue;
        }

        private void SetQuantityOfItens(int quantityOfItens)
        {
            if (quantityOfItens <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantityOfItens), "The quantity of itens must be greater than zero.");
            }

            _quantityOfItens = quantityOfItens;
        }

        private void SetProduct(Product product)
        {
            Product = product;
            Code = product.Code;
        }

        private void SetTotalValue()
        {
            TotalValue = _unitValue * _quantityOfItens;
        }

        private void Validate()
        {
            IsValid = _unitValue > 0 && _quantityOfItens > 0 && Product.IsValid;
        }
    }
}