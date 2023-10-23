using ReStore.Data;
using ReStore.DTOs;
using ReStore.Entities;

namespace ReStore.Repository.Interfaces
{
    public interface IBasketRepository
    {
        Task<BasketDto> AddItem(int quantity, int productId, string buyerId);

        Task RemoveItem(string buyerId,int productId, int quantity);

        Task<BasketDto> GetBasket(string buyerId);

    }
}
