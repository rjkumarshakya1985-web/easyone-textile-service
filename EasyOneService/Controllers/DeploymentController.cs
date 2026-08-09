using Microsoft.AspNetCore.Mvc;

namespace EasyOne.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeploymentController : ControllerBase
    {
        [HttpGet("version")]
        public IActionResult GetVersion()
        {
            return Ok(new
            {
                version = "2.0",
                message = "Deployment Slot Demo"
            });
        }
    }
}