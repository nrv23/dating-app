using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.interfaces;
using AutoMapper;
using API.DTOs;

namespace API.Controllers
{
    [Authorize] // una vez que en el archivo program.cs se configura el middleware para autenticacion y autorizacion, se debe poner esta
    // anotacion en cada clase donde los servicios sean privados, en el nivel de mayor jerarquia para indicar que
    // todos los servicios de la clase son privados
    public class UsersController : BaseApiController
    {
        private readonly IUserRespository _userRepository;
        private readonly IMapper _iMapper;

        public UsersController(IUserRespository userRepository, IMapper iMapper)
        {
            _userRepository = userRepository;
            _iMapper = iMapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
        {
            var users = await _userRepository.GetUsersAsync();
            var response = _iMapper.Map<IEnumerable<MemberDTO>>(users); // convierte los datos con el automapper a instancia del MemberDTO
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MemberDTO>> GetById(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            var response = _iMapper.Map<MemberDTO>(user); // convierte los datos con el automapper a instancia del MemberDTO
            return response;
        }
    }
}