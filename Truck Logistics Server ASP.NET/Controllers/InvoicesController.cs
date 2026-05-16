using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TrucksLogisticsServerAPI.Data;
using TrucksLogisticsServerAPI.Models;
using TrucksLogisticsServerAPI.Models.Helping_Models;

namespace TrucksLogisticsServerAPI.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : Controller
    {
        private readonly DataContext _datacontext;

        public InvoicesController(DataContext datacontext)
        {
            _datacontext = datacontext;
        }

        [HttpGet("Get_Invoices_Stats")]
        public async Task<ActionResult<InvoicesStats>> GetInvoicesStats()
        {
            var unpaidCount = await _datacontext.Invoices.Where(x => x.Status != "paid").CountAsync();
            var overdueCount = await _datacontext.Invoices.Where(x => x.DueDate < DateTime.Now && x.Status != "paid").CountAsync();

            var stats = new InvoicesStats
            {
                Unpaid_Count = unpaidCount,
                Overdue_Count = overdueCount
            };

            return Ok(stats);
        }

        [HttpGet("GeneratePDF/{id}")]
        public async Task<ActionResult> GetInvoice(int id)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var invoice = await _datacontext.Invoices.Include(x => x.Client).Include(x => x.Job).FirstOrDefaultAsync(i => i.ID == id);

            if (invoice == null)
            {
                return NotFound("Invoice not found.");
            }
            var document = new InvoiceDocument(invoice);

            var pdfbytes = document.GeneratePdf();

            return File(pdfbytes, "application/pdf", $"invoice-{id}.pdf");
        }

        [HttpGet("Get_All_Invoices")]
        public async Task<ActionResult<List<Invoice>>> Get_All_Invoices()
        {
            var allInvoices = await _datacontext.Invoices.Include(i => i.Client).Include(x => x.Job).ToListAsync();
            if (allInvoices != null)
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
            if (InvoiceToAdd != null)
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

        [HttpPut("Update_Invoice/{id}")]
        public async Task<ActionResult<Invoice>> Update_Invoice(int id, Invoice InvoiceToUpdate)
        {
            var invoice = await _datacontext.Invoices.FirstAsync(x => x.ID == id);

            if(invoice == null)
            {
                return NotFound("Invoice not found.");
            }

            invoice.Status = InvoiceToUpdate.Status;

            _datacontext.Invoices.Update(invoice);

            await _datacontext.SaveChangesAsync();

            return Ok("Successfully updated Invoice.");
        }
        

        [HttpDelete("Delete_Invoice/{id}")]

        public async Task<ActionResult> Delete_Invoice(int id)
        {
            var invoiceToDelete = await _datacontext.Invoices.FindAsync(id);
            if (invoiceToDelete != null)
            {
                _datacontext.Invoices.Remove(invoiceToDelete);
                await _datacontext.SaveChangesAsync();
                return Ok("Successfully deleted Invoice.");
            }
            else
            {
                return NotFound("Invoice to delete not found.");
            }



        }
    }
}
