using Microsoft.AspNetCore.Mvc;

namespace SC.DevChallenge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PricesController : ControllerBase
    {
        [HttpGet("average")]
        public string Average()
        {
            return "I'm dummy controller";
        }
    }
}
