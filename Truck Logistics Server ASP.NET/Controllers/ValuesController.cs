using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TrucksLogisticsServerAPI.Controllers;
using TrucksLogisticsServerAPI.Data;
using TrucksLogisticsServerAPI.Migrations;
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
        [Authorize(Roles = "admin,user")]
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

    }
}
