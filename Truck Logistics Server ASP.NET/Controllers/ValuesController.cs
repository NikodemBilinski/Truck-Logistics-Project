using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrucksLogisticsServerAPI.Controllers;
using TrucksLogisticsServerAPI.Data;
using TrucksLogisticsServerAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TrucksLogisticsServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public ValuesController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        // get trucks from database

        [HttpGet("Get_Trucks")]

        public async Task <ActionResult<List<Truck>>> GetTrucks()
        {
            Console.WriteLine("GetTrucks Requested");

            return Ok(await _dataContext.Trucks.ToListAsync());
        }


        // insert new truck to table

        [HttpPost("Post_Truck")]
        public async Task<ActionResult<Truck>> AddTruck(Truck truck)
        {
            
            _dataContext.Trucks.Add(truck);

            await _dataContext.SaveChangesAsync();

            Console.WriteLine("Truck added: " + truck.Id + ", " + truck.Owner + ", " + truck.IsBusy + ", " + truck.Capacity);

            return Ok(await _dataContext.Trucks.ToListAsync());

            
        }

        // insert new user username, password and role


        [HttpPost("Post_UserLogin")]

        public async Task<ActionResult<Users>> AddUserLogin(Users userslogin)
        {
            var userslist = await _dataContext.Users.ToListAsync();

            if(userslogin.Username != null && userslogin.Password != null)
            {
                //if role is empty - set it to user
                if(userslogin.Role == string.Empty)
                {
                    userslogin.Role = "user";
                }

                //set role to lowercase - easier checking
                userslogin.Role = userslogin.Role.ToLower();

                //check the role string
                if (userslogin.Role != "user" && userslogin.Role != "admin")
                {

                    return BadRequest("Error: Invalid role for user (use admin or user)."); 
                }

                // check if the username already exist
                if (userslist.Any(x => x.Username == userslogin.Username))
                {
                    return BadRequest("Error: Username already taken.");
                }


                // add new user
                _dataContext.Users.Add(userslogin);

                await _dataContext.SaveChangesAsync();

                Console.WriteLine("User added: " + userslogin.Username + ", " + userslogin.Password + ", " + userslogin.Role);

                return Ok(await _dataContext.Users.ToListAsync());

            }
            else
            {
                return BadRequest("Error: Username and password cannot be null.");
            }
        }
    }
}
