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

        [HttpGet("{ID}/{tri}")]
        public async Task<ActionResult<List<MPresupuesto>>> GetD(int ID,int tri)
        {
            var result  = await function.GetD(ID,tri);
            return Ok(result);
        }
        [HttpGet]
        public async Task<ActionResult<List<MPresupuesto>>> GetP(int ID, decimal Presupuesto)
        {
            var result = await function.GetP(ID,Presupuesto);
            return Ok(result);
        }


    }
}
