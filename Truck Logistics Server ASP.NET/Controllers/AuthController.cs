using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrucksLogisticsServerAPI.Models;

namespace TrucksLogisticsServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            Console.WriteLine("Request to login for user: " + model.Username);


            // In a real application, you should validate against a database or an identity provider.
            if (model.Username == "admin" && model.Password == "admin")
            {
                Console.WriteLine("Login successful for user: " + model.Username);
                var token = "fake-jwt-token-for-admin";
                return Ok(new { Token = token });
            }
            else
            {
                Console.WriteLine("Login unsuccessful for user: " + model.Username);
                return Unauthorized();
            }
        }
    }
}
