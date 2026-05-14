using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TrucksLogisticsServerAPI.Data;
using TrucksLogisticsServerAPI.Models;

namespace TrucksLogisticsServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrucksController : Controller
    {
        private readonly DataContext _dataContext;

        public TrucksController(DataContext datacontext)
        {
            _dataContext = datacontext;
        }

        //HTTP GETS
        [Authorize(Roles = "admin")]
        [HttpGet("Get_Trucks")]

        public async Task<ActionResult<List<Truck>>> GetTrucks()
        {
            Console.WriteLine("GetTrucks: Requested.");
            Console.WriteLine("GetTrucks: Returning All Trucks.");
            return Ok(await _dataContext.Trucks.ToListAsync());
        }

        //HTTP POST
        [Authorize(Roles = "admin")]
        [HttpPost("Add_Truck")]
        public async Task<ActionResult<Truck>> AddTruck(Truck TruckToAdd)
        {
            Console.WriteLine("AddTruck: Requested To Add Truck.");

            var trucklist = await _dataContext.Trucks.ToListAsync();

            if (TruckToAdd != null)
            {
                if (!trucklist.Any(x => x.Name == TruckToAdd.Name))
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

        //HTTP PUTS
        [Authorize(Roles = "admin")]
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

        //HTTP DELETES 
        [Authorize(Roles = "admin")]
        [HttpDelete("Delete_Truck/{ID}")]
        public async Task<ActionResult<Truck>> DeleteTruck(int ID)
        {
            Console.WriteLine("DeleteTruck: Request To Delete Truck With ID: " + ID);

            var TruckToDelete = await _dataContext.Trucks.FindAsync(ID);

            if (TruckToDelete != null)
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
