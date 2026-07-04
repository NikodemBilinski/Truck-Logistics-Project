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
        public async Task<ActionResult<InvoicesResponse>> GetInvoicesStats()
        {
            var invoicescount = await _datacontext.Invoices.CountAsync();
            var unpaidCount = await _datacontext.Invoices.Where(x => x.Status != "paid").CountAsync();
            var overdueCount = await _datacontext.Invoices.Where(x => x.DueDate < DateTime.Now && x.Status != "paid").CountAsync();

            var stats = new InvoicesResponse
            {
                Invoices_Count = invoicescount,
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

        [HttpGet("Get_Invoices_Page/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<InvoicesResponse>> GetInvoicesPage(int pageNumber, int pageSize, string clientname = null, string jobname = null, DateTime? duedate = null, DateTime? issuedate = null)
        {
            var query = _datacontext.Invoices
                .Include(i => i.Client)
                .Include(x => x.Job)
                .AsQueryable();

            if(!string.IsNullOrEmpty(clientname))
            {
                query = query.Where(i => i.Client.Name.Contains(clientname));
            }
            if(!string.IsNullOrEmpty(jobname))
            {
                query = query.Where(i => i.Job.Name.Contains(jobname));
            }
            if(duedate != null)
            {
                query = query.Where(i => i.DueDate.Date == duedate.Value.Date);
            }
            if(issuedate != null)
            {
                query = query.Where(i => i.IssueDate.Date == issuedate.Value.Date);
            }

            int invoicescount = await query.CountAsync();

            List<Invoice> invoices = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            return Ok(new InvoicesResponse
            {
                Invoices = invoices,
                Invoices_Count = invoicescount
            });
        }

        [HttpPost("Add_Invoice")]
        public async Task<ActionResult<Invoice>> Add_Invoice(Invoice InvoiceToAdd)
        {
            #region validation
            
            if(InvoiceToAdd == null)
            {
                return BadRequest("Invoice to add is null!");
            }
            if(InvoiceToAdd.IssueDate > InvoiceToAdd.DueDate)
            {
                return BadRequest("Invoice issue date is invalid");
            }
            if(InvoiceToAdd.DueDate <= DateTime.Now || InvoiceToAdd.DueDate < InvoiceToAdd.IssueDate)
            {
                return BadRequest("Inovice due date is invalid");
            }
            if(InvoiceToAdd.NetAmount <= 0)
            {
                return BadRequest("Invoice net amount is invalid");
            }
            if(InvoiceToAdd.VatRate <= 0 || InvoiceToAdd.VatRate > 100)
            {
                return BadRequest("Invoice vat rate is invalid");
            }
            if(InvoiceToAdd.Client == null)
            {
                return BadRequest("Invoice Client is null");
            }
            if(InvoiceToAdd.Job == null)
            {
                return BadRequest("Invoice Job is null");
            }
            #endregion
            
            _datacontext.Invoices.Add(InvoiceToAdd);
            await _datacontext.SaveChangesAsync();
            return Ok("Successfully added new Invoice.");

            
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
