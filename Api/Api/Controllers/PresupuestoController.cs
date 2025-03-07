using Api.Date;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace Api.Controllers
{
    [ApiController]
    [Route("Presupuesto")]
    public class PresupuestoController : Controller
    {
        private DPresupuesto function = new DPresupuesto();

        [HttpGet("{ID}")]
        public async Task<ActionResult<List<MPresupuesto>>> Get(int ID)
        {
            var result  = await function.Get(ID);
            return Ok(result);
        }
    }
}
