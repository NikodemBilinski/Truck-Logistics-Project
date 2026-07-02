using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Diagnostics;
using TrucksLogisticsClient.Models.Helping_Models;
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
        [Authorize(Roles = "admin")]
        [HttpGet("Get_Trucks_Stats")]
        
        public async Task<ActionResult<TrucksResponse>> GetTrucksStats()
        {
            var Truckcount = await _dataContext.Trucks.CountAsync();
            var AvailableTrucksCount = await _dataContext.Trucks.Where(x => x.IsBusy == false).CountAsync();
            var BusyTrucksCount = await _dataContext.Trucks.Where(x => x.IsBusy == true).CountAsync();
            var DiffrentBrandsCount = await _dataContext.Trucks.Select(x => x.brand).Distinct().CountAsync();

            var TrucksStats = new TrucksResponse()
            {
                Truck_Count = Truckcount,
                AvaiableTrucks_Count = AvailableTrucksCount,
                BusyTrucks_Count = BusyTrucksCount,
                DiffrentBrands_Count = DiffrentBrandsCount
            };

            return Ok(TrucksStats);

        }

        [HttpGet("Get_Trucks_Page/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<TrucksResponse>> GetTrucksPage(int pageNumber, int pageSize, string name = null, string status = null, string brand = null)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("Error: Page number and page size must be greater than 0.");
            }

            var query = _dataContext.Trucks.AsQueryable();

            if(!string.IsNullOrEmpty(name))
            {
                query = query.Where(x => x.Name.Contains(name));
            }
            if (!string.IsNullOrEmpty(status))
            {
                status.ToLower();
                if (status == "busy")
                {
                    query = query.Where(x => x.IsBusy == true);
                }
                if (status == "available")
                {
                    query = query.Where(x => x.IsBusy == false);
                }
            }
            if(!string.IsNullOrEmpty(brand))
            {
                query = query.Where(x => x.brand.Contains(brand));
            }
            
            int truckscount = await query.CountAsync();

            var truckspage = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new TrucksResponse
            {
                Trucks = truckspage,
                Truck_Count = truckscount
            });
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

            #region validation
            var truck = await _dataContext.Trucks.FindAsync(id);
            if (truck == null)
            {
                Console.WriteLine("UpdateTruck: Error, Truck with the specified ID not found.");
                return NotFound("Error: Truck with the specified ID not found.");
            }
            if(updatedTruck == null)
            {
                return BadRequest("Truck to update is null");
            }
            if(string.IsNullOrEmpty(updatedTruck.Name))
            {
                return BadRequest("Truck Name is empty!");
            }
            if(string.IsNullOrEmpty(updatedTruck.brand))
            {
                return BadRequest("Truck Brand is empty!");
            }
            if(updatedTruck.Capacity <= 0)
            {
                return BadRequest("Truck Capacity is below zero!");
            }
            #endregion

            // Update truck properties
            truck.Name = updatedTruck.Name;
            truck.brand = updatedTruck.brand;
            truck.Capacity = updatedTruck.Capacity;
            truck.IsBusy = updatedTruck.IsBusy;
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
