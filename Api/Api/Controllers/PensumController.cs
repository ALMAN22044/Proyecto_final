using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/Pensum")]
    public class PensumController : Controller
    {
        DPensum Function = new DPensum();
        [HttpGet]
        public async Task<ActionResult> Get()    
        {
            var result = await Function.Get();
            return Ok(result);
        }
    }
}
