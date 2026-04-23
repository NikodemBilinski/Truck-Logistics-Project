using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
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
            Console.WriteLine("GetTrucks: Requested.");
            Console.WriteLine("GetTrucks: Returning All Trucks.");
            return Ok(await _dataContext.Trucks.ToListAsync());
        }

        [HttpGet("Get_User_By_ID/{ID}")]

        public async Task<ActionResult<Users>> GetUserByID(int ID)
        {
            Console.WriteLine("GetUserByID: Requested.");
            var user = await _dataContext.Users.FindAsync(ID);
            if (user == null)
            {
                return NotFound("Error: User with the specified ID not found.");
            }
            Console.WriteLine("GetUserByID: Returning User: " + user.Username + ".");
            return Ok(user);
        }

        [HttpGet("Get_All_Users")]

        public async Task<ActionResult<IEnumerable<Users>>> GetAllUsers()
        {
            Console.WriteLine("GetAllUsers: Requested.");
            Console.WriteLine("GetAllUsers: Returning All Users.");
            var allusers = await _dataContext.Users.Include(u => u.AssignedTrucks).Include(u => u.Languages).ToListAsync();

            //await _dataContext.Users.Include(u => u.AssignedTrucks).Include(u => u.Languages).ToListAsync();
            return Ok(allusers);
        }

        [HttpGet("Get_Languages")]

        public async Task<ActionResult<List<Language>>> GetLanguages()
        {
            var languages = await _dataContext.Languages.ToListAsync();

            if (languages != null)
            {
                return Ok(languages);
            }
            return BadRequest("Error: No languages found in database.");
        }

        // HTTP POSTS

        // insert new truck to table

        [HttpPost("Add_Truck")]
        public async Task<ActionResult<Truck>> AddTruck(Truck TruckToAdd)
        {
            Console.WriteLine("AddTruck: Requested To Add Truck.");

            var trucklist = await _dataContext.Trucks.ToListAsync();

            if(TruckToAdd != null)
            {
                if(!trucklist.Any(x => x.Name == TruckToAdd.Name))
                {
                    _dataContext.Trucks.Add(TruckToAdd);

                    await _dataContext.SaveChangesAsync();

                    Console.WriteLine("AddTruck: Added Truck: " + TruckToAdd.Id + ". " + TruckToAdd.Name + ", To Database.");

                    return Ok("Successfully added truck.");
                }
                else
                {
                    return BadRequest("Error: Name already taken.");
                }
                
            }
            else
            {
                return BadRequest("Error: Truck cannot be null.");
            }


        }

        // insert new user username, password and role

        [HttpPost("Add_User")]

        public async Task<ActionResult<Users>> AddUser(Users UserToAdd)
        {
            Console.WriteLine("AddUser: Requested To Add User: " + UserToAdd.Username + ".");
            var userslist = await _dataContext.Users.ToListAsync();

            UserToAdd.Role.ToLower();

            if(UserToAdd.Role == string.Empty || (UserToAdd.Role != "admin" && UserToAdd.Role != "user"))
            {
                UserToAdd.Role = "user";
            }

            // check if the username already exist
            if (userslist.Any(x => x.Username == UserToAdd.Username))
            {
                Console.WriteLine("AddUser: Error, Username Already Taken.");
                return BadRequest("Error: Username already taken.");
            }

            _dataContext.Users.Add(UserToAdd);

            await _dataContext.SaveChangesAsync();

            Console.WriteLine("AddUser: Added User: " + UserToAdd.ID + ". " + UserToAdd.Username + ", To Database.");
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
            Console.WriteLine("UpdateUser: Request to update user with ID: " + id);

            var user = await _dataContext.Users.FindAsync(id);
            if (user == null)
            {
                Console.WriteLine("UpdateUser: Error, User with the specified ID not found.");
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

            Console.WriteLine("UpdateUser: User Updated.");
            return Ok("User updated successfully.");
        }

        [HttpPut("Update_Truck/{id}")]
        public async Task<ActionResult<Truck>> UpdateTruck(int id, Truck updatedTruck)
        {
            Console.WriteLine("UpdateTruck: Request to update truck with ID: " + id);
            var truck = await _dataContext.Trucks.FindAsync(id);
            if (truck == null)
            {
                Console.WriteLine("UpdateTruck: Error, Truck with the specified ID not found.");
                return NotFound("Error: Truck with the specified ID not found.");
            }
            // Update truck properties
            truck.Name = updatedTruck.Name;
            truck.brand = updatedTruck.brand;
            truck.Capacity = updatedTruck.Capacity;
            truck.IsBusy = false;
            await _dataContext.SaveChangesAsync();
            Console.WriteLine("UpdateTruck: Truck Updated.");
            return Ok("Truck updated successfully.");
        }

        // HTTP DELETES

        [HttpDelete("Delete_User/{ID}")]

        public async Task<ActionResult<Users>> DeleteUser(int ID)
        {
            Console.WriteLine("DeleteUser: Request To Delete User With ID: " + ID);
            var UserToDelete = await _dataContext.Users.FindAsync(ID);

            if(UserToDelete != null)
            {
                _dataContext.Users.Remove(UserToDelete);
                await _dataContext.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("DeleteUser: Error, User Not Found.");
                return BadRequest("Error: User not Found.");
            }

            Console.WriteLine("DeleteUser: Deleted User From Database.");
            return Ok("Successfully deleted user from database");
        }

        [HttpDelete("Delete_Truck/{ID}")]
        public async Task<ActionResult<Truck>> DeleteTruck(int ID)
        {
            Console.WriteLine("DeleteTruck: Request To Delete Truck With ID: " + ID);

            var TruckToDelete = await _dataContext.Trucks.FindAsync(ID);

            if(TruckToDelete != null)
            {
                _dataContext.Trucks.Remove(TruckToDelete);
                await _dataContext.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("DeleteTruck: Error, Truck Not Found.");
                return BadRequest("Error: Truck not Found.");
            }
            Console.WriteLine("DeleteTruck: Deleted Truck From Database.");
            return Ok();
        }
    }
}
