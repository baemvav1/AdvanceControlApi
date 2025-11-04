using Clases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OnlineController : ControllerBase
    {
        [HttpGet(Name = "OnlineCheck")]
        public bool Get()
        {
            return true;
        }
    }
}