using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrucksLogisticsServerAPI.Data;
using TrucksLogisticsServerAPI.Models;
using BCrypt.Net;
using TrucksLogisticsServerAPI.Models.Helping_Models;

namespace TrucksLogisticsServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly DataContext _dataContext;

        public UsersController(DataContext datacontext)
        {
            _dataContext = datacontext;
        }

        //HTTP GETS
        [Authorize(Roles = "admin, user")]
        [HttpGet("Get_User_By_ID/{ID}")]

        public async Task<ActionResult<Users>> GetUserByID(int ID)
        {
            Console.WriteLine("GetUserByID: Requested.");
            var user = await _dataContext.Users.Include(x => x.AssignedTrucks).Include(x => x.AssignedJobs).Include(x => x.Languages).FirstOrDefaultAsync(x => x.ID == ID);
            if (user == null)
            {
                return NotFound("Error: User with the specified ID not found.");
            }
            Console.WriteLine("GetUserByID: Returning User: " + user.Username + ".");
            return Ok(user);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("Get_All_Users")]

        public async Task<ActionResult<IEnumerable<Users>>> GetAllUsers()
        {
            Console.WriteLine("GetAllUsers: Requested.");

            var allusers = await _dataContext.Users.Include(u => u.AssignedTrucks).Include(x => x.AssignedJobs).Include(u => u.Languages).ToListAsync();
            Console.WriteLine("GetAllUsers: Returning All Users.");
            return Ok(allusers);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("Get_Users_Stats")]
        public async Task<ActionResult<UsersResponse>> GetUsersStats()
        {
            var Alluserscount = await _dataContext.Users.CountAsync();
            var AvaiableUsersCount = await _dataContext.Users.Where(x=> x.isBusy == false).CountAsync();
            var BusyUsersCount = await _dataContext.Users.Where(x => x.isBusy == true).CountAsync();
            var AdminCount = await _dataContext.Users.Where(x => x.Role == "admin").CountAsync();
            var UserCount = await _dataContext.Users.Where(x => x.Role == "user").CountAsync();

            var usersstats = new UsersResponse()
            {
                Users_Count = Alluserscount,
                AvaiableUsers_Count = AvaiableUsersCount,
                BusyUsers_Count = BusyUsersCount,
                Admin_Count = AdminCount,
                User_Count = UserCount
                
            };

            return Ok(usersstats);
        }

        [HttpGet("Get_Users_Page/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<UsersResponse>> GetUsersPage(int pageNumber = 1, int pageSize = 10, string status = null, string username = null)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("Error: Page number and page size must be greater than 0.");
            }

            var query = _dataContext.Users.Include(x => x.AssignedTrucks)
                .Include(x => x.AssignedJobs)
                .Include(x => x.Languages)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                status = status.ToLower();

                if (status == "available")
                {
                    query = query.Where(x => x.isBusy == false);
                }
                if(status == "busy")
                {
                    query = query.Where(x => x.isBusy == true);
                }    
            }

            if(!string.IsNullOrEmpty(username))
            {
                username = username.ToLower();

                query = query.Where(x => x.Username.Contains(username));
            }

            int totalusers = await query.CountAsync();

            var usersPage = await query
                .Skip((pageNumber - 1 ) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new UsersResponse()
            {
                Users = usersPage,
                Users_Count = totalusers
            });
        }
        //HTTP POST 
        [Authorize(Roles = "admin")]
        [HttpPost("Add_User")]

        public async Task<ActionResult<Users>> AddUser(Users UserToAdd)
        {
            Console.WriteLine("AddUser: Requested To Add User: " + UserToAdd.Username + ".");

            #region validation
            if (UserToAdd.Role == string.Empty)
            {
                UserToAdd.Role = "user";
            }

            UserToAdd.Role = UserToAdd.Role.ToLower();

            if ( (UserToAdd.Role != "admin" && UserToAdd.Role != "user"))
            {
                UserToAdd.Role = "user";
            }
            if(string.IsNullOrEmpty(UserToAdd.FirstName))
            {
                return BadRequest("First Name is Empty!");
            }
            if(string.IsNullOrEmpty(UserToAdd.LastName))
            {
                return BadRequest("Last Name is Empty!");
            }
            if(UserToAdd.Age <= 0 || UserToAdd.Age > 120)
            {
                return BadRequest("Age is either empty or not realistic!");
            }
            if(string.IsNullOrEmpty(UserToAdd.Username))
            {
                return BadRequest("Username is Empty!");
            }
            if(string.IsNullOrEmpty(UserToAdd.Password) || UserToAdd.Password.Length <= 6)
            {
                return BadRequest("Password is Empty or too short (at least 6 characters).");
            }

            // check if the username already exist
            bool usernameExists = await _dataContext.Users.AnyAsync(x => x.Username == UserToAdd.Username);

            if (usernameExists)
            {
                Console.WriteLine("AddUser: Error, Username Already Taken.");
                return BadRequest("Error: Username already taken.");
            }

            #endregion

            UserToAdd.Password = BCrypt.Net.BCrypt.HashPassword(UserToAdd.Password);

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

                userslogin.Password = BCrypt.Net.BCrypt.HashPassword(userslogin.Password);
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


        //HTTP PUTS
        [Authorize(Roles = "admin,user")]
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

            //skidibi toilet lepszy sposub
            bool ismatching = await _dataContext.Users.AnyAsync(x => x.Username == updatedUser.Username && x.ID != updatedUser.ID);

            if (ismatching)
            {
                Console.WriteLine("UpdateUser: Error, There is already user with that Username.");
                return BadRequest("Error: There is already user with that Username.");
            }
            if(user.Password != updatedUser.Password)
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(updatedUser.Password);
            }

            // Update user properties
            user.Username = updatedUser.Username;
            user.Role = updatedUser.Role;
            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Age = updatedUser.Age;
            user.isBusy = updatedUser.isBusy;

            await _dataContext.SaveChangesAsync();

            Console.WriteLine("UpdateUser: User Updated.");
            return Ok("User updated successfully.");
        }


        [Authorize(Roles = "admin,user")]
        [HttpPut("Update_User_Languages/{id}")]
        public async Task<ActionResult<Language>> UpdateUserLanguages(int id, List<Language> updatedLanguages)
        {
            Console.WriteLine("UpdateUserLanguages: Request to update languages for user with ID: " + id);

            // load relation (users.languages with Languages table in database)
            var user = await _dataContext.Users.Include(x => x.Languages).FirstOrDefaultAsync(x => x.ID == id);

            if (user == null)
            {
                Console.WriteLine("UpdateUserLanguages: Error, User with the specified ID not found.");
                return NotFound("Error: User with the specified ID not found.");
            }

            // get only selectedlanguages ids from updatedlanguages list from client
            var selectedLanguageIds = updatedLanguages.Select(l => l.Id).ToList();

            //get all matching languages ids from database and store them in order to add them to user.languages
            var languagesfromdb = await _dataContext.Languages.Where(l => selectedLanguageIds.Contains(l.Id)).ToListAsync();


            // Update languages
            user.Languages.Clear();
            foreach (var language in languagesfromdb)
            {
                user.Languages.Add(language);
            }
            await _dataContext.SaveChangesAsync();
            Console.WriteLine("UpdateUserLanguages: User Languages Updated.");
            return Ok("User languages updated successfully.");
        }


        [Authorize(Roles = "admin,user")]
        [HttpPut("Update_User_Trucks/{id}")]
        public async Task<ActionResult<Truck>> UpdateUserTrucks(int id, List<Truck> updatedTrucks)
        {
            var user = _dataContext.Users.Include(x => x.AssignedTrucks).FirstOrDefault(x => x.ID == id);

            if (user == null)
            {
                Console.WriteLine("UpdateUserTrucks: Error, User with the specified ID not found.");
                return NotFound("Error: User with the specified ID not found.");
            }

            //get ids from updatedtrucks
            var selectedtrucksids = updatedTrucks.Select(x => x.Id).ToList();
            //match ids from updatedtrucks with trucks in database
            //list of them 
            var trucksfromdb = await _dataContext.Trucks.Where(x => selectedtrucksids.Contains(x.Id)).ToListAsync();


            user.AssignedTrucks.Clear();
            foreach (var truck in trucksfromdb)
            {
                user.AssignedTrucks.Add(truck);
            }

            await _dataContext.SaveChangesAsync();

            Console.WriteLine("UpdateUserTrucks: User Trucks Updated.");
            return Ok("User trucks updated successfully.");
        }

        //HTTP DELETES
        [Authorize(Roles = "admin")]
        [HttpDelete("Delete_User/{ID}")]

        public async Task<ActionResult<Users>> DeleteUser(int ID)
        {
            Console.WriteLine("DeleteUser: Request To Delete User With ID: " + ID);
            var UserToDelete = await _dataContext.Users.FindAsync(ID);

            if (UserToDelete != null)
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
    }
}
