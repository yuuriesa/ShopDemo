namespace CustomerManagement.Utils
{
    public class DateVerify
    {
        public static bool CheckIfTheDateIsGreaterThanToday(DateTime datetime)
        {
            var dateNow = DateTime.UtcNow;

            if (datetime.ToUniversalTime().Date > dateNow.Date)
            {
                return true;
            }

            return false;
        }
    }
}