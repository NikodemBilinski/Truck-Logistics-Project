using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrucksLogisticsServerAPI.Data;
using TrucksLogisticsServerAPI.Models;
using Microsoft.AspNetCore.Authorization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using TrucksLogisticsServerAPI.Models.Helping_Models;

namespace TrucksLogisticsServerAPI.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : Controller
    {
        private readonly DataContext _dataContext;

        public ClientsController(DataContext datacontext)
        {
            _dataContext = datacontext;
        }

        //HTTP GETS 

        [HttpGet("Get_Clients")]
        public async Task<ActionResult<List<Client>>> GetClients()
        {
            var clients = await _dataContext.Clients.ToListAsync();

            if (clients != null)
            {
                return Ok(clients);
            }
            return BadRequest("Error: No clients found in database.");
        }

        [HttpGet("Get_Client_By_ID/{ID}")]
        public async Task<ActionResult<Client>> GetClientByID(int id)
        {
            var client = await _dataContext.Clients.FirstOrDefaultAsync(x => x.ID == id);

            if (client != null)
            {
                return Ok(client);
            }
            return BadRequest("Error: No client with that id was found");
        }

        [HttpGet("Get_Clients_Page/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Client>>> GetClientsPage(int pageNumber, int pageSize)
        {
            var clients = await _dataContext.Clients
                .Include(x => x.Invoices)
                .Include(x => x.Jobs)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(clients);
        }

        [HttpGet("Get_Clients_Stats")]
        public async Task<ActionResult<ClientsResponse>> GetClientsStats()
        {
            var clientcount = await _dataContext.Clients.CountAsync();

            return Ok(new ClientsResponse()
            {
                TotalClients = clientcount
            });

        }


        //HTTP POSTS

        [HttpPost("Add_Client")]
        public async Task<ActionResult<Client>> AddClient(Client ClientToAdd)
        {
            if (ClientToAdd != null)
            {
                _dataContext.Clients.Add(ClientToAdd);
                await _dataContext.SaveChangesAsync();
                Console.WriteLine("AddClient: Added Client: " + ClientToAdd.ID + ". " + ClientToAdd.Name + ", To Database.");
                return Ok("Successfully added new client: " + ClientToAdd.Name);
            }
            else
            {
                Console.WriteLine("AddClient: Error Adding Client.");
                return BadRequest("Error: Client cannot be null");
            }

        }

        //HTTP PUTS

        [HttpPut("Update_Client/{ID}")]

        public async Task<ActionResult<Job>> UpdateClient(int id, Client updatedClient)
        {
            if (updatedClient == null)
            {
                Console.WriteLine("UpdateClient: Error, Updated Client is null.");
                return BadRequest("Error: Updated Client is null.");
            }

            var client = await _dataContext.Clients.FirstAsync(x => x.ID == id);

            if (client == null)
            {
                Console.WriteLine("UpdateClient: Error, Client with the specified ID not found.");
                return NotFound("Error: Client with the specified ID not found.");
            }

            client.Name = updatedClient.Name;
            client.NIP = updatedClient.NIP;
            client.Address = updatedClient.Address;
            client.City = updatedClient.City;
            client.PostalCode = updatedClient.PostalCode;
            client.Country = updatedClient.Country;
            client.Phone = updatedClient.Phone;
            client.Email = updatedClient.Email;

            await _dataContext.SaveChangesAsync();
            Console.WriteLine("UpdateClient: Client Updated.");
            return Ok("Client Updated Successfully.");
        }

        //HTTP DELETES

        [HttpDelete("Delete_Client/{ID}")]

        public async Task<ActionResult<Client>> DeleteClient(int id)
        {
            var ClientToDelete = await _dataContext.Clients.FindAsync(id);

            if(ClientToDelete == null)
            {
                Console.WriteLine("DeleteClient: Error, Client to delete is null.");
                return NotFound("Error, Client to delete is null.");
            }

            _dataContext.Clients.Remove(ClientToDelete);

            await _dataContext.SaveChangesAsync();

            return Ok("Successfully deleted user.");
        }

    }
}
