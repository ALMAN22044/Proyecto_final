using Api.Date;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace Api.Controllers
{
    [ApiController]
    [Route("pryapi/pensum")]
    public class PensumController : Controller
    {
        DPensum function = new DPensum();

        [HttpGet]
        public async Task<ActionResult<List<MPensum>>> GetPensum() { 

            var result = await function.GetPensum();
            return result;
        }
    }
}
