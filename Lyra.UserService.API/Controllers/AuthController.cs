using Lyra.UserService.API.DataAccess;
using Lyra.UserService.API.DTOs;
using Lyra.UserService.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Lyra.UserService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(UserDbContext context, IConfiguration configuration)
        {
            this._context = context;
            this._configuration = configuration;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult<User>> Register(RegisterRequest request)
        {
            var user = await this._context.Users.FirstOrDefaultAsync(x => x.Email.Equals(request.Email));
            if (user != null)
            {
                return BadRequest("User already exists!");
            }
            var newUser = new User();
            newUser.Email = request.Email;
            newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            newUser.Name = request.Name;
            await this._context.Users.AddAsync(newUser);
            await this._context.SaveChangesAsync();
            return Ok(newUser);
        }

        [HttpPost]
        [Route("AccessToken")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            var user = await this._context.Users.FirstOrDefaultAsync(x => x.Email.Equals(request.Email));
            if (user == null) return NotFound("User not found");
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return BadRequest("Incorrect Password");
            }
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var securityKey = this._configuration.GetSection("Keys:SecurityKey").Value;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey!));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "Lyra", 
                audience: "Lyra", 
                claims: userClaims, 
                expires: DateTime.Now.AddDays(1), 
                signingCredentials: cred);

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            var response = new LoginResponse()
            {
                //AccessToken = jwtToken,
                UserId = user.Id,
                Name = user.Name
            };

            HttpContext.Response.Cookies.Append("lyra_access_token", jwtToken, new CookieOptions() 
            { 
                SameSite = SameSiteMode.None, Secure = true, HttpOnly = true, Path="/", Domain = null, Expires = DateTime.Now.AddDays(1) 
            });

            var userDetails = JsonConvert.SerializeObject(new { name = user.Name, userId = user.Id }, Formatting.Indented);
            HttpContext.Response.Cookies.Append("lyra_user", userDetails, new CookieOptions() { 
                SameSite = SameSiteMode.None, Secure = true, Path="/", Domain = null, Expires = DateTime.Now.AddDays(1)
            });

            return Ok(response);
        }
    }
}
