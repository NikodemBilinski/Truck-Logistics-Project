using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrucksLogisticsServerAPI.Data;

namespace TrucksLogisticsServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly DataContext _datacontext;

        public UsersController(DataContext datacontext)
        {
            _datacontext = datacontext;
        }
    }
}
