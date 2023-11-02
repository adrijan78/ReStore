using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using ReStore.Data;
using ReStore.DTOs;
using ReStore.Entities;
using ReStore.Exceptions;
using ReStore.Repository.Interfaces;

namespace ReStore.Repository
{
    public class BasketRepository : IBasketRepository
    {
        private readonly RestoreDbContext _context;
        private readonly IMapper _mapper;

        public BasketRepository(RestoreDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BasketDto> AddItem(int quantity, int productId, string buyerId)
        {
            Basket basket = await CreateBasket(buyerId);

            //Check if the product is already in the basket
            var existingBasketItem = basket.Items.FirstOrDefault(x => x.ProductId == productId);

            //If yes increase the quatnity
            if (existingBasketItem is not null)
            {
                existingBasketItem.Quantity += quantity;
            }
            else
            {
                //If not create the basket item
                var newBasketItem = new BasketItem
                {
                    Quantity = quantity,
                    ProductId = productId,

                };


                //Add to basket 
                basket.Items.Add(newBasketItem);

                

            }


            _context.Baskets.Update(basket);   

            await _context.SaveChangesAsync();

            return _mapper.Map<BasketDto>(await GetBasket(buyerId));

           
        }

        public async Task<BasketDto> GetBasket(string buyerId)
        {

            var basketDto =  await _context.Baskets
                .Where(x => x.BuyerId == buyerId)
                .ProjectTo<BasketDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
            if(basketDto is null)
            {
               return null;
            }


            return basketDto;
           
        }

        public  void RemoveBasket(int basketId)
        {
            var basket = _context.Baskets.FirstOrDefault(x => x.Id == basketId);
            if(basket == null)
            {
                throw new NotFoundException("Basket not found");
            }  

             _context.Baskets.Remove(basket);
            _context.SaveChanges();

        }

        public async Task RemoveItem(string buyerId, int productId, int quantity)
        {
            var basket= await CreateBasket(buyerId);
            if(basket is null)
            {
                throw new NotFoundException("Basket not found");
            };

            var existingBasketItem = basket.Items.FirstOrDefault(x=>x.ProductId== productId);
            
            if(existingBasketItem is null)
            {
                throw new NotFoundException("Basket item not found");
            }

            existingBasketItem.Quantity -= quantity;

            if(existingBasketItem.Quantity <=0) {
                basket.Items.Remove(existingBasketItem);
            }

            _context.Baskets.Update(basket);

            await _context.SaveChangesAsync();




        }

        //Made this function only for Account Controller!!!1
        public async Task Update(BasketDto basketDto)
        {
            var basket = await _context.Baskets.FirstOrDefaultAsync(x=>x.Id == basketDto.Id);
             basket.BuyerId = basketDto.BuyerId;
            _context.Baskets.Update(basket);
            await _context.SaveChangesAsync();
 
        }

        private async Task<Basket> CreateBasket(string buyerId)
        {
            //Check if the basket exists
            var basket = await _context.Baskets.Include(x => x.Items).FirstOrDefaultAsync(x => x.BuyerId.Equals(buyerId));
            //If not create one
            if (basket is null)
            {
                basket = new Basket
                {
                    BuyerId = buyerId,
                };
            }

            return basket;
        }
    }
}
