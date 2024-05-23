using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Store.Models;
using System.Diagnostics.Eventing.Reader;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Store.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
       

        public AccountController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Userview model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.Email, Address = model.Address, Name = model.Name };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var tokenString = GenerateJwtToken(user); // Генерация JWT токена
                    return Ok(new { Token = tokenString, User = new { Name = user.Name, Email = user.Email, Address = user.Address } }); // Возвращаем токен и информацию о пользователе
                }
                else
                {
                    return BadRequest(result.Errors); // Возвращаем 400 BadRequest с описанием ошибок
                }
            }
            return BadRequest(ModelState); // Возвращаем 400 BadRequest с моделью состояния
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginView model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var tokenString = GenerateJwtToken(user); // Генерация JWT токена
                    return Ok(new { Token = tokenString, User = new { Name = user.Name, Email = user.Email, Address = user.Address } }); // Возвращаем токен и информацию о пользователе
                }
                else
                {
                    return BadRequest("Invalid login attempt."); // Возвращаем 400 BadRequest с сообщением об ошибке
                }
            }
            return BadRequest(ModelState); // Возвращаем 400 BadRequest с моделью состояния
        }
        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("C3F7A8E5B6D9E2C0A4F8E1D3B6A9D1E3B7D2C9F0A8D4C7B6A8D9F3E2C4A5F7D3A8F2A4D5E8A9D3E1B7C8D1A5E6A7F0B7E8A3D6E1B4F9A7E0B3D2F8E3D1F2A6D8E2C3A9F1E4C8B6A8E3C7D5F8B5C9E4D3B8E2A5F0A9C2D7B4E9F1A8E2D0B5A9F6C7D9A3E0C4B6E7A2F1D8C6A9E2C1D0F7B4D6E0F9C7A8D1B6E5A7D0F1B7C2E3F7D9A4F2E8C1A9F3B7E4C1A3F8A5B2C4F7A8B1C2E3D0F2C9A7D1E8F0C2D4F1C6A8D0C4A5E1B7C8D5B1D7A3E4D2B5A8F0E3C5B7D9C1F8E4B3C6F9A7E1F4C9A6F0C8B2A5C0E7B9D2F6B3E9A2D5F3C8A9B6D2C5E6D4A7C2F0A3C9E2D3A5F7D2A9C6E5B1A3F9C2B0D6C8F0E4A1B8C4A6B9E3F1C7D9B2C3A8F4E7A2D6E4A1C5E9B3C6D0A5C9F6D7A4E0F8B1C3E6B4C0F3E5D9A6E1B8C4E2D7B9F0A2C5F2A6C9E7A0B2F4C6D3A2E5D7B6C3F8A0E2F6C4A3D9E0C6A7E4B0D7F3C8A2B4E5A1D9E7C4F0B2C5F8D2B6F1C3E9B4A5C7F9E1C2F6E3A0C8F2E6C5A1F4B3D0F6A2C7E8B0D5F9C3E0A4C2F5A8C6D9B0E2F1B7D0C3F4A5E6F8C9B1D2A4C0B7F9A6B3C5A7B0D3C1A9E7D5F8B1E4A2C8F3A0B6C4E5A1B8F0E7C2D4F9A3E8D0A6B5F7A8C9E1D2F4C0B9E3A5B7F6A2E9C3B0A7D8A4B2C6E0A9F1C8B5E2A3F9B4D7E6B1C0E4D1A8C7D3A1B9F8D4A0C2F6B8A9C1B3F5A4C8E1D0B6C7F3D9B0E3A6C5E9A8C0D2F8A3D4B7A5C4D6F9C3B2D8E7B4C1D5E2B8F0C6D7A0B1E5C9A7B3C2A9D4E6C0B5A2F7A4D0E3F5B9C8A3D6B2F8A1E4A6B7F3E0A8C4F9B0E2D8B6F7C5E9D1F0B3C7D3F8B4C1A5E7B9D5C2A0E6C8F1D2A6F7C4B\r\n"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "my_issuer",
                audience: "my_audience",
                claims: new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                },
                expires: DateTime.UtcNow.AddHours(1), // Устанавливаем время жизни токена (1 час)
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
