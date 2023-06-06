using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")] // se puede dejar como esta para que tome el nombre de controlador por defecto o asignarle un nombre personalizado
 
    public class BaseApiController: ControllerBase
    {

    }
}