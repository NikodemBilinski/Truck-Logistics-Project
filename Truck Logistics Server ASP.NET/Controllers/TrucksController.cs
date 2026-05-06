using Microsoft.AspNetCore.Mvc;
using TrucksLogisticsServerAPI.Data;

namespace TrucksLogisticsServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrucksController : Controller
    {
        private readonly DataContext _datacontext;

        public TrucksController(DataContext datacontext)
        {
            _datacontext = datacontext;
        }
    }
}
