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

            if ((user == null) || (!BCrypt.Net.BCrypt.Verify(model.Password, user.Password)))
            {
                return BadRequest("Invalid username or password");
            }

            // zmiana klucza na bajty
            var bytestoken = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            // utworzenie klucza z tych bajtów
            var signingkey = new SymmetricSecurityKey(bytestoken);

            // utworzenie "ustawien" do signingcredentials, czyli algorytmu i klucza
            var creds = new SigningCredentials(signingkey, SecurityAlgorithms.HmacSha256);

            //deklaracja tokena, claims - dane w tokenie, expires - czas waznosci, signingcredentials - ustawienia do podpisu
            var token = new JwtSecurityToken(
                claims: new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                },
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: creds
                );

            // hashowanie tokena do stringa
            var tokenstring = new JwtSecurityTokenHandler().WriteToken(token);

            Console.WriteLine("Login: Successfully Logged in " + model.Username + ".");

            // wysylka w paczusce
            return Ok(new { user, token = tokenstring });
        }
    }
}
