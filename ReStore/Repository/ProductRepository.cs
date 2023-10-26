using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ReStore.Data;
using ReStore.DTOs;
using ReStore.Entities;
using ReStore.Exceptions;
using ReStore.Repository.Interfaces;

namespace ReStore.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly RestoreDbContext _context;

        public ProductRepository(RestoreDbContext context)
        {
            _context = context;
        }

        public async Task<ProductsWithPagination> GetAllProducts(string orderBy,string searchTerm, string filterByBrand, string filterByType,int pageNumber,int pageSize)
        {
            var products = _context.Products.AsQueryable();


            products = orderBy switch
            {
                "price" => products.OrderBy(p => p.Price),
                "priceDesc" => products.OrderByDescending(p => p.Price),
                _ => products.OrderBy(p => p.Name),
            };

            if (!searchTerm.IsNullOrEmpty())
            {
                searchTerm = searchTerm.ToLower();
                products = products.Where(p => p.Name.Contains(searchTerm));
            }

            products = filterFunction(filterByBrand, filterByType, products);


            //pagination
            var count = await products.CountAsync();

            var productWithPagination = new ProductsWithPagination
            {
                Products = await products.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(),
                PageSize =pageSize,
                TotalCount= count,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize),
                CurrentPage = pageNumber
            };
            

           

            return productWithPagination;

        }

        public async Task<Product> GetProductById(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                throw new NotFoundException("Product with that Id was not found");
            }
            return product;
        }



        static IQueryable<Product> filterFunction(string filterByBrand, string filterByType, IQueryable<Product> products)
        {
            var brandList = new List<string>();
            var typeList = new List<string>();

            if (!string.IsNullOrEmpty(filterByBrand))
                brandList.AddRange(filterByBrand.ToLower().Split(','));

            if (!string.IsNullOrEmpty(filterByType))
                typeList.AddRange(filterByType.ToLower().Split(','));

            products = products.Where(p => brandList.Count == 0 || brandList.Contains(p.Brand.ToLower()));
            products = products.Where(p => typeList.Count == 0 || typeList.Contains(p.Type.ToLower()));
            return products;
        }

        public async Task<object> GetFilters()
        {
            var brands =await _context.Products.Select(p => p.Brand).Distinct().ToListAsync();
            var types =await _context.Products.Select(p => p.Type).Distinct().ToListAsync();

            return new {brands,types };
        }
    }
}
