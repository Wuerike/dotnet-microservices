using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [ApiController]
    [Route("api/platforms")]
    public class PlatformController : ControllerBase
    {
        public PlatformController()
        {
            
        }

        [HttpPost]
        public ActionResult TestaConexão()
        {
            Console.WriteLine("---> Teste de conexão");

            return Ok("---> Teste de conexão");
        }


    }
}

