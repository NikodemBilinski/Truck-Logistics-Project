using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrucksLogisticsServerAPI.Data;
using TrucksLogisticsServerAPI.Models;

namespace TrucksLogisticsServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : Controller
    {
        private readonly DataContext _datacontext;

        public InvoicesController(DataContext datacontext)
        {
            _datacontext = datacontext;
        }

        [HttpGet("Get_All_Invoices")]
        public async Task<ActionResult<List<Invoice>>> Get_All_Invoices()
        {
            var allInvoices = await _datacontext.Invoices.ToListAsync();
            if(allInvoices != null)
            {
                return Ok(allInvoices);
            }
            else
            {
                return BadRequest("No Invoices were found in the database.");
            }
        }

        [HttpPost("Add_Invoice")]
        public async Task<ActionResult<Invoice>> Add_Invoice(Invoice InvoiceToAdd)
        {
            if(InvoiceToAdd != null)
            {
                _datacontext.Invoices.Add(InvoiceToAdd);
                await _datacontext.SaveChangesAsync();
                return Ok("Successfully added new Invoice.");
            }
            else
            {
                return BadRequest("Invoice to add is null.");
            }
        }
    }
}
