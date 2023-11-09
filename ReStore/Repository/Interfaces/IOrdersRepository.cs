using ReStore.DTOs;
using ReStore.Entities.OrderAggregate;

namespace ReStore.Repository.Interfaces
{
    public interface IOrdersRepository
    {

        Task<List<ReturnedOrderDto>> GetOrders(string buyerId);

        Task<ReturnedOrderDto> GetOrderById(int id, string buyerId);
        Task<Order> GetOrderByPaymentIntendId(string paymentIntendId);

        Task<int> CreateOrder(OrderDto orderDto, string buyerId);

        void SaveChanges();

    }
}
