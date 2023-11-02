using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReStore.DTOs;
using ReStore.Entities;
using ReStore.Repository.Interfaces;

namespace ReStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketsController : ControllerBase
    {
        private readonly IBasketRepository _basketRepository;
        public BasketsController(IBasketRepository basketRepository)
        {
            _basketRepository = basketRepository;
        }



        [HttpGet(Name ="GetBasket")]
        public async Task<ActionResult<BasketDto>> GetBasket()
        {
            try
            {
                var buyerId = User.Identity.Name ?? Request.Cookies["buyerId"];
                if (string.IsNullOrEmpty(buyerId))
                {
                    Response.Cookies.Delete("buyerId");
                    return BadRequest("Something went wrong");
                }
                var basketDto = await _basketRepository.GetBasket(buyerId);
                return Ok(basketDto);
            }
            catch (Exception ex)
            {

                return NotFound(ex.Message);
            }
        } 


        [HttpPost]
        public async Task<ActionResult<BasketDto>> AddItems(int quantity, int productId)
        {
            try
            {
               var buyerId= Request.Cookies["buyerId"]?? GenerateBuyerId();
               var basketDto=await _basketRepository.AddItem(quantity, productId, buyerId);
               return Ok(basketDto);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
              
            }

           
        }


        [HttpDelete]
        public async Task<ActionResult> RemoveItem(int productId,int quantity)
        {
            try
            {
                var buyerId = Request.Cookies["buyerId"];
                await _basketRepository.RemoveItem(buyerId, productId, quantity);
                return Ok("Item removed successfully!");
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }




        private string GenerateBuyerId()
        {
            var buyerId = User.Identity?.Name;
            if (string.IsNullOrEmpty(buyerId))
            {
                buyerId = Guid.NewGuid().ToString();
                var cookieOpt = new CookieOptions { IsEssential = true, Expires = DateTime.Now.AddDays(30) };
                Response.Cookies.Append("buyerId", buyerId, cookieOpt);
            }
                       
            return buyerId;
        }
    }
}
