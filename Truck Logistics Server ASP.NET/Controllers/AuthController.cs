using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrucksLogisticsServerAPI.Data;
using TrucksLogisticsServerAPI.Models;

namespace TrucksLogisticsServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _datacontext;

        public AuthController(DataContext datacontext)
        {
            _datacontext = datacontext;
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

            Console.WriteLine("Login: Successfully Logged in " + model.Username + ".");

            return Ok(user);
        }
    }
}
