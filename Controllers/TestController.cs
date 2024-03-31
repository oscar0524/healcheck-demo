using Microsoft.AspNetCore.Mvc;

namespace healcheck_demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController(ILogger<TestController> logger) : ControllerBase
    {

        [HttpGet]
        public string Get()
        {
            logger.LogInformation("Test OK");
            return "OK";
        }

    }
}
