﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<User> _userManager;

        public ProductsController(RestoreDbContext context, IProductRepository productRepository, UserManager<User> userManager)
        {
            _context = context;
            _productRepository = productRepository;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<ProductsWithPagination>> GetAll(string orderBy,
            string searchTerm, string filterByBrand,
            string filterByType, int pageNumber = 1,
            int pageSize=6)
        {
            //var user1 = new User
            //{
            //    UserName ="Bob",
            //    Email="bob@hotmail.com"
            //};

            //await _userManager.CreateAsync(user1,"Pa$$w0rd");
            //await _userManager.AddToRoleAsync(user1, "Member");

            //var user2 = new User
            //{
            //    UserName = "Admin",
            //    Email = "admin@hotmail.com"
            //};

            //await _userManager.CreateAsync(user2, "Pa$$w0rd");
            //await _userManager.AddToRolesAsync(user2,new[] { "Member", "Admin" });


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
