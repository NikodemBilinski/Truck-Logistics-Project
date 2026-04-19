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

        [HttpGet]

        public async Task <ActionResult<List<Truck>>> GetTrucks()
        {
            Console.WriteLine("GetTrucks Requested");

            return Ok(await _dataContext.Trucks.ToListAsync());
        }

        [HttpPost]
        public async Task<ActionResult<Truck>> AddTruck(Truck truck)
        {
            
            _dataContext.Trucks.Add(truck);

            await _dataContext.SaveChangesAsync();

            Console.WriteLine("Truck added: " + truck.Id + ", " + truck.Owner + ", " + truck.IsBusy + ", " + truck.Capacity);

            return Ok(await _dataContext.Trucks.ToListAsync());

            
        }
    }
}
