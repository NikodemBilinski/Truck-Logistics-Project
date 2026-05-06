using Microsoft.AspNetCore.Mvc;
using TrucksLogisticsServerAPI.Data;

namespace TrucksLogisticsServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : Controller
    {
        private readonly DataContext _datacontext;

        public ClientsController(DataContext datacontext)
        {
            _datacontext = datacontext;
        }
    }
}
