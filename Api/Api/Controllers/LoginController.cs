using Api.Date;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;

namespace Api.Controllers
{
    [ApiController]
    [Route("Login")]
    public class LoginController : Controller
    {

        [HttpPost]
        public async Task<ActionResult<List<MLoginResult>>> Post([FromBody] MLogin parameters)
        {
            var function = new  DLogin();
            var result = await function.Login(parameters);
            if (result == null) 
                return BadRequest("Usuario/Contraseña Incorrecta");
            else
                return Ok(result);
        }
    }
}
