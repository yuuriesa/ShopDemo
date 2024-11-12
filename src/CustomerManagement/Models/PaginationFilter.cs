namespace CustomerManagement.Models
{
    public class PaginationFilter
    {
        public int PageNumber  { get; set; }
        public int PageSize  { get; set; }

        public PaginationFilter(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize > 10 ? 10 : pageSize;
        }
    }
}