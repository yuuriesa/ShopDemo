using CustomerManagement.DTO;
using Microsoft.AspNetCore.Mvc;

namespace CustomerManagement.Utils
{

    public class CustomerValidator
    {
        public bool VerifyDateOfBirth(DateTime customerDateOfBirth)
        {
            var dateNow = DateTime.UtcNow;

            if (customerDateOfBirth.ToUniversalTime().Date > dateNow.Date)
            {
                return true;
            }

            return false;
        }
    }
}

