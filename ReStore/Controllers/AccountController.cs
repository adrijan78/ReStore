using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReStore.DTOs;
using ReStore.Entities;
using ReStore.Repository.Interfaces;
using ReStore.Services.Interfaces;

namespace ReStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IBasketRepository _basketRepository;

        public AccountController(UserManager<User> userManager, ITokenService tokenService, IBasketRepository basketRepository)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _basketRepository = basketRepository;
        }


        [HttpPost("login")]
        public async Task<ActionResult<UserTokenDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if(user is null || !await _userManager.CheckPasswordAsync(user,loginDto.Password))
            {
                return Unauthorized();
            }

            var userBasket = await _basketRepository.GetBasket(loginDto.Username);
            var anonBasket = await _basketRepository.GetBasket(Request.Cookies["buyerId"]);

            if(anonBasket != null) 
            {
                if(userBasket != null)
                {
                    _basketRepository.RemoveBasket(userBasket.Id);
                    
                } 
                anonBasket.BuyerId = user.UserName;
                Response.Cookies.Delete("buyerId");
                await _basketRepository.Update(anonBasket);
            }

            var userToken = new UserTokenDto
            {
                Email = user.Email,
                Token = await _tokenService.GenerateToken(user),
                Basket = anonBasket ?? userBasket,
            };

            return Ok(userToken);
        }


        [HttpPost("register")]
        public async Task <IActionResult> Register (RegisterDto registerDto)
        {
            var user = new User { UserName = registerDto.Username,Email = registerDto.Email };
            var result  = await _userManager.CreateAsync(user,registerDto.Password);
            if (!result.Succeeded) {
                
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return ValidationProblem();

            }

            await _userManager.AddToRoleAsync(user, "Member");

            return StatusCode(201);
        }

        [Authorize]
        [HttpGet("currentUser")]
        public async Task<ActionResult<UserTokenDto>> GetCurrentUser()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            return new UserTokenDto { Email = user.Email,Token=await _tokenService.GenerateToken(user) };

        }

    }
}
