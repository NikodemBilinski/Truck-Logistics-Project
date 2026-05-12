using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TrucksLogisticsServerAPI.Data;
using TrucksLogisticsServerAPI.Models;

namespace TrucksLogisticsServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _datacontext;
        private readonly IConfiguration _configuration;
        public AuthController(DataContext datacontext, IConfiguration configuration)
        {
            _datacontext = datacontext;
            _configuration = configuration;
        }


        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            Console.WriteLine("Login: Request To Login For User: " + model.Username + ".");

            var userslist = _datacontext.Users.ToList();
            //look for matching username

            var user = userslist.FirstOrDefault(u => u.Username == model.Username);

            if ((user == null) || (user.Password != model.Password))
            {

                return BadRequest("Invalid username or password");
            }

            //var token = "fake-token";

            var bytestoken = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var signingkey = new SymmetricSecurityKey(bytestoken);

            var creds = new SigningCredentials(signingkey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                },
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: creds
                );

            var tokenstring = new JwtSecurityTokenHandler().WriteToken(token);

            Console.WriteLine("Login: Successfully Logged in " + model.Username + ".");

            return Ok(new { user, token = tokenstring });
        }
    }
}
