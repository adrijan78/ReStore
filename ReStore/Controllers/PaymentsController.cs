using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReStore.DTOs;
using ReStore.Entities;
using ReStore.Entities.OrderAggregate;
using ReStore.Repository.Interfaces;
using ReStore.Services.Interfaces;
using Stripe;

namespace ReStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {

        private readonly IPaymentService _paymentsService;
        private readonly IBasketRepository _basketRepository;
        private readonly IOrdersRepository _ordersRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public PaymentsController(IPaymentService paymentsService, IBasketRepository basketRepository, IConfiguration configuration, IOrdersRepository ordersRepository)
        {
            _paymentsService = paymentsService;
            _basketRepository = basketRepository;
            _configuration = configuration;
            _ordersRepository = ordersRepository;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<BasketDto>> CreateOrUpdatePaymentIntent()
        {
            var basketDto = await _basketRepository.GetBasket(User.Identity.Name);
            if (basketDto == null)
            {
                return NotFound();
            }

         


            var intent = await _paymentsService.CreateOrUpdatePaymentIntent(basketDto);

            if(intent == null)
            {
                return BadRequest("Problem creating payment intent");
            }

            basketDto.PaymentIntentId = basketDto.PaymentIntentId ?? intent.Id;
            basketDto.ClientSecret = basketDto.ClientSecret ?? intent.ClientSecret;

            await _basketRepository.Update(basketDto);

            return Ok(basketDto);

        }


        [HttpPost("webhook")]
        public async Task<ActionResult> StripeWebHook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _configuration["StripeSettings:WhSecret"]);

            var charge = (Charge)stripeEvent.Data.Object;

            var order = await _ordersRepository.GetOrderByPaymentIntendId(charge.PaymentIntentId);

            if (charge.Status == "succeeded") order.OrderStatus = OrderStatus.PaymentReceived;

            _ordersRepository.SaveChanges();

            return new EmptyResult();
        }


    }
}
