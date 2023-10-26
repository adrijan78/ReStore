using ReStore.DTOs;
using ReStore.Entities;

namespace ReStore.Repository.Interfaces
{
    public interface IProductRepository
    {
        public Task<ProductsWithPagination> GetAllProducts(string orderBy,string searchTerm,string filterByBrand,string filterByType, int pageNumber, int pageSize);
        public Task<Product> GetProductById(int id);

        public Task<Object> GetFilters();
    }
}
