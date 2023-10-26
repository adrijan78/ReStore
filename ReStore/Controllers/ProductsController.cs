using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReStore.Data;
using ReStore.DTOs;
using ReStore.Entities;
using ReStore.Repository.Interfaces;

namespace ReStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly RestoreDbContext _context;
        private readonly IProductRepository _productRepository;

        public ProductsController(RestoreDbContext context, IProductRepository productRepository)
        {
            _context = context;
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<ActionResult<ProductsWithPagination>> GetAll(string orderBy, string searchTerm, string filterByBrand, string filterByType, int pageNumber = 1, int pageSize=6)
        {
            return Ok(await _productRepository.GetAllProducts(orderBy, searchTerm, filterByBrand, filterByType, pageNumber, pageSize));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            return await _productRepository.GetProductById(id);
        }

        [HttpGet("filters")]
        public async Task<IActionResult> GetFilters()
        {
            var filters = await _productRepository.GetFilters();
            return Ok(filters);
        }
    }
}
