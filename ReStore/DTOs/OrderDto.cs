using ReStore.Entities.OrderAggregate;

namespace ReStore.DTOs
{
    public class OrderDto
    {
        public bool IsAddressSaved { get; set; }
        public ShippingAddress ShippingAddress { get; set; }
    }
}
