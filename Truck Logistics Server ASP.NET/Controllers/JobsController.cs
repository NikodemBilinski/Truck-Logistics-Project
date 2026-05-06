using Microsoft.AspNetCore.Mvc;
using TrucksLogisticsServerAPI.Data;

namespace TrucksLogisticsServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : Controller
    {
        private readonly DataContext _datacontext;

        public JobsController(DataContext datacontext)
        {
            _datacontext = datacontext;
        }
    }
}
