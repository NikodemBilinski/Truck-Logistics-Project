using Microsoft.AspNetCore.Mvc;
using TrucksLogisticsServerAPI.Data;

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
    }
}
