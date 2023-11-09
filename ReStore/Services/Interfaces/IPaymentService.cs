using ReStore.DTOs;
using ReStore.Entities;
using Stripe;

namespace ReStore.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentIntent> CreateOrUpdatePaymentIntent(BasketDto basket);
    }
}
