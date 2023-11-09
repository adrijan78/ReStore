using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReStore.DTOs;
using ReStore.Entities.OrderAggregate;
using ReStore.Repository.Interfaces;

namespace ReStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersRepository _ordersRepository;

        public OrdersController(IOrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
        }



        [HttpGet]

        public async Task<ActionResult<List<ReturnedOrderDto>>> GetOrders()
        {
            return await _ordersRepository.GetOrders(User.Identity.Name);
        }

        [HttpGet("{id}", Name ="GetOrderById")]
        public async Task<ActionResult<ReturnedOrderDto>> GetOrderById(int id)
        {
            return await _ordersRepository.GetOrderById(id, User.Identity.Name);
        }


        [HttpPost]
        public async Task<ActionResult<int>> CreateOrder(OrderDto orderDto)
        {
            var orderId =await _ordersRepository.CreateOrder(orderDto, User.Identity.Name);
            

            return CreatedAtRoute("GetOrderById", new {id=orderId },orderId);
        }
        




    }
}
