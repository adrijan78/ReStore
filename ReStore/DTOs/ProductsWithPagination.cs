using ReStore.Entities;

namespace ReStore.DTOs
{
    public class ProductsWithPagination
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public List<Product> Products { get; set; }
    }
}
