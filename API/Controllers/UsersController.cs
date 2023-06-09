using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.interfaces;
using API.DTOs;

namespace API.Controllers
{
    [Authorize] // una vez que en el archivo program.cs se configura el middleware para autenticacion y autorizacion, se debe poner esta
    // anotacion en cada clase donde los servicios sean privados, en el nivel de mayor jerarquia para indicar que
    // todos los servicios de la clase son privados
    public class UsersController : BaseApiController
    {
        private readonly IUserRespository _userRepository;
    

        public UsersController(IUserRespository userRepository)
        {
            _userRepository = userRepository;
   
        }

        /*  

        [HttpGet("{id}")]
        public async Task<ActionResult<MemberDTO>> GetById(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            var response = _iMapper.Map<MemberDTO>(user); // convierte los datos con el automapper a instancia del MemberDTO
            return response;
        }
        
        */
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDTO>> GetMemberByUsername(string username)
        {
            return await _userRepository.GetMemberByUsernameAsync(username);
        }


        [HttpGet]
        public async Task<ActionResult<MemberDTO>> GetMembersAsync()
        {
            return Ok(await _userRepository.GetMembersAsync());
        }
    }
}