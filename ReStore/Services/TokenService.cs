using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ReStore.Entities;
using ReStore.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ReStore.Services
{
    public class TokenService : ITokenService
    {

        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public TokenService(UserManager<User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<string> GenerateToken(User user)
        {
            var claims = new List<Claim> {

                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.NameIdentifier,user.Id)

            };


            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles) {
                
                    claims.Add(new Claim(ClaimTypes.Role, role));

            }


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTSettings:TokenKey"]));

            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha512);

            var tokenOptions = new JwtSecurityToken(     
                    issuer:null,
                    audience:null,
                    claims:claims,
                    expires: DateTime.UtcNow.AddDays(7),
                    signingCredentials: creds
                );


            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);


        }
    }
}
