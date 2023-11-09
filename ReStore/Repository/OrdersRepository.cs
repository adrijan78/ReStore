using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using ReStore.Data;
using ReStore.DTOs;
using ReStore.Entities;
using ReStore.Entities.OrderAggregate;
using ReStore.Exceptions;
using ReStore.Repository.Interfaces;

namespace ReStore.Repository
{
    public class OrdersRepository : IOrdersRepository
    {
        private readonly RestoreDbContext _context;
        private readonly IMapper _mapper;


        public OrdersRepository(RestoreDbContext context, IBasketRepository basketRepository, IProductRepository productRepository, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int> CreateOrder(OrderDto orderDto,string buyerId)
        {
            var basket =  await _context.Baskets
                .Include(b=>b.Items)
                .Where(x => x.BuyerId == buyerId)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("There's no basket for this user");

            var items = new List<OrderItems>();

            foreach(var item in basket.Items)
            {
                var productItem = await _context.Products.FindAsync(item.ProductId);
                var itemOrdered = new ProductItemOrdered
                {
                    ProductId = productItem.Id,
                    Name = productItem.Name,
                    PictureUrl = productItem.PictureUrl,
                };

                var orderItem = new OrderItems
                {
                    ItemOrdered = itemOrdered,
                    Price = productItem.Price,
                    Quantity = item.Quantity,
                    
                    

                };


                items.Add(orderItem);

                productItem.QuantityInStock -= item.Quantity;


            };

            var subtotal = items.Sum(x => x.Price*x.Quantity);
            var deliveryFee = subtotal > 10000 ? 500 : 0;

            var order = new Order
            {
                OrderItems = items,
                BuyerId = buyerId,
                ShippingAddress = orderDto.ShippingAddress,
                Subtotal = subtotal,
                DeliveryFee = deliveryFee,
                PaymentIntentId = basket.PaymentIntentId
                
            };

             _context.Orders.Add(order);
             _context.Baskets.Remove(basket);

            if(orderDto.IsAddressSaved)
            {
                var user = await _context.Users.Include(a=>a.Address).FirstOrDefaultAsync(x => x.UserName == buyerId);

               var address = new UserAddress
                {
                    FullName = orderDto.ShippingAddress.FullName,
                    Address1 = orderDto.ShippingAddress.Address1,
                    Address2 = orderDto.ShippingAddress.Address2,
                    City= orderDto.ShippingAddress.City,
                    State = orderDto.ShippingAddress.State,
                    Zip = orderDto.ShippingAddress.Zip,
                    Country = orderDto.ShippingAddress.Country,
                };
                user.Address = address;

              //  _context.Update(user);
            };
            await _context.SaveChangesAsync();

            return order.Id;


        }

        public async Task<ReturnedOrderDto> GetOrderById(int id,string buyerId)
        {
            return await _context.Orders.Where(o=>o.Id == id && o.BuyerId ==buyerId).ProjectTo<ReturnedOrderDto>(_mapper.ConfigurationProvider).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<Order> GetOrderByPaymentIntendId(string paymentIntendId)
        {
            return await _context.Orders.FirstOrDefaultAsync(x => x.PaymentIntentId == paymentIntendId);
        }

        public async Task<List<ReturnedOrderDto>> GetOrders(string buyerId)
        {
            return await _context.Orders
                .Where(o=>o.BuyerId == buyerId)
                .ProjectTo<ReturnedOrderDto>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
