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

        // HTTP GETS 

        // get trucks from database

        [HttpGet("Get_Trucks")]

        public async Task<ActionResult<List<Truck>>> GetTrucks()
        {
            Console.WriteLine("GetTrucks Requested");

            return Ok(await _dataContext.Trucks.ToListAsync());
        }

        [HttpGet("Get_User_By_ID/{ID}")]

        public async Task<ActionResult<Users>> GetUserByID(int ID)
        {
            var user = await _dataContext.Users.FindAsync(ID);
            if (user == null)
            {
                return NotFound("Error: User with the specified ID not found.");
            }
            return Ok(user);
        }

        [HttpGet("Get_All_Users")]

        public async Task<ActionResult<IEnumerable<Users>>> GetAllUsers()
        {
            var allusers = await _dataContext.Users.ToListAsync();
            return Ok(allusers);
        }


        // HTTP POSTS

        // insert new truck to table

        [HttpPost("Post_Truck")]
        public async Task<ActionResult<Truck>> AddTruck(Truck truck)
        {

            _dataContext.Trucks.Add(truck);

            await _dataContext.SaveChangesAsync();

            Console.WriteLine("Truck added: " + truck.Id + ", " + truck.Name + ", " + truck.IsBusy + ", " + truck.Capacity);

            return Ok(await _dataContext.Trucks.ToListAsync());


        }

        // insert new user username, password and role

        [HttpPost("Add_User")]

        public async Task<ActionResult<Users>> AddUser(Users UserToAdd)
        {
            var userslist = await _dataContext.Users.ToListAsync();

            UserToAdd.Role.ToLower();

            if(UserToAdd.Role == string.Empty || (UserToAdd.Role != "admin" && UserToAdd.Role != "user"))
            {
                UserToAdd.Role = "user";
            }

            // check if the username already exist
            if (userslist.Any(x => x.Username == UserToAdd.Username))
            {
                return BadRequest("Error: Username already taken.");
            }

            _dataContext.Users.Add(UserToAdd);

            await _dataContext.SaveChangesAsync();

            return Ok("Successfully added new user: " + UserToAdd.Username);

        }

        [HttpPost("Post_User_Swagger")]

        public async Task<ActionResult<Users>> AddUserLogin(Users userslogin)
        {
            var userslist = await _dataContext.Users.ToListAsync();

            if (userslogin.Username != null && userslogin.Password != null)
            {
                //if role is empty - set it to user
                if (userslogin.Role == string.Empty)
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

                foreach (var language in userslogin.Languages)
                {
                    // "Attach" mówi EF: "Ten obiekt już jest w bazie, nie próbuj go dodawać ponownie, 
                    // po prostu użyj jego ID do stworzenia relacji".
                    _dataContext.Languages.Attach(language);
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


        // HTTP PUTS

        [HttpPut("Update_User/{id}")]

        public async Task<ActionResult<Users>> UpdateUser(int id, Users updatedUser)
        {
            Console.WriteLine("Request to update user with ID: " + id);

            var user = await _dataContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("Error: User with the specified ID not found.");
            }


            // Update user properties
            user.Username = updatedUser.Username;
            user.Password = updatedUser.Password;
            user.Role = updatedUser.Role;
            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Age = updatedUser.Age;
            user.isBusy = updatedUser.isBusy;

            // Update languages
            user.Languages.Clear();
            foreach (var language in updatedUser.Languages)
            {
                _dataContext.Languages.Attach(language);
                user.Languages.Add(language);
            }

            // Update trucks
            user.AssignedTrucks.Clear();
            foreach (var truck in updatedUser.AssignedTrucks)
            {
                _dataContext.Trucks.Attach(truck);
                user.AssignedTrucks.Add(truck);
            }
            await _dataContext.SaveChangesAsync();

            Console.WriteLine("User updated");
            return Ok("User updated successfully.");
        }

        // HTTP DELETES

        [HttpDelete("Delete_User/{ID}")]

        public async Task<ActionResult<Users>> DeleteUser(int ID)
        {
            var UserToDelete = await _dataContext.Users.FindAsync(ID);

            if(UserToDelete != null)
            {
                _dataContext.Users.Remove(UserToDelete);
                await _dataContext.SaveChangesAsync();
            }
            else
            {
                return BadRequest("Error: User not Found");
            }

            return Ok("Successfully deleted user from database");
        }
    }
}
