namespace CustomerManagement.Models
{
    public class Product
    {
        public int Id { get; private set; }
        private string _code { get; set; }
        private string _name { get; set; }
        public string Code => _code;
        public string Name => _name;

        public bool IsValid { get; private set; }

        //constructors
        //private contructor
        private Product(){}
        private Product(int id, string code, string name)
        {
            Id = id;
            _code = code;
            _name = name;
            Validate();
        }

        //public methods
        public static Product RegisterNew(string code, string name)
        {
            var product = new Product();
            product.SetCode(code);
            product.SetName(name);
            product.Validate();

            return product;
        }

        public static Product SetExistingInfo(int id, string code, string name)
        {
            var product = new Product(id: id, code: code, name: name);
            product.Validate();

            return product;
        }

        // private methods
        private void SetCode(string code)
        {
            if (code.Length > 40)
            {
                throw new ArgumentOutOfRangeException(nameof(code), "The code length cannot exceed 40 characters.");
            }

            _code = code;
        }
        private void SetName(string name)
        {
            if (name.Length > 40)
            {
                throw new ArgumentOutOfRangeException(nameof(name), "The name length cannot exceed 40 characters.");
            }

            _name = name;
        }
        private void Validate()
        {
            IsValid = _code.Length <= 40 && _code.Length > 0 && _name.Length <= 40 && _name.Length > 0;
        }
    }
}
