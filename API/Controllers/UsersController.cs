using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.interfaces;
using API.DTOs;
using AutoMapper;
using API.Extensions;
using API.Entities;

namespace API.Controllers
{
    [Authorize] // una vez que en el archivo program.cs se configura el middleware para autenticacion y autorizacion, se debe poner esta
    // anotacion en cada clase donde los servicios sean privados, en el nivel de mayor jerarquia para indicar que
    // todos los servicios de la clase son privados
    public class UsersController : BaseApiController
    {
        private readonly IUserRespository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUserRespository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _photoService = photoService;
        }

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

        [HttpPut]
        public async Task<ActionResult> updateUser(memberUpdateDto memberUpdateDto)
        {

            var user = await this._userRepository.GetUserByUsernameAsync(User.getUsername());

            if (user == null) return NotFound();

            _mapper.Map(memberUpdateDto, user); // Lo que hace el mapper en este caso es que sobreescribe los valores de las propiedades 
            // del usuario con los datos que trae el data que es del tipo memberUpdateDto, actualiza las propiedades 
            // que comparten
            var updated = await this._userRepository.SaveAllAsync();

            if (updated) return NoContent();

            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]

        public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
        {

            var user = await this._userRepository.GetUserByUsernameAsync(User.getUsername());

            if (user == null) return NotFound();

            var result = await this._photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 0) photo.isMain = true; // si tiene solo una foto en el perfil

            user.Photos.Add(photo);

            if (await this._userRepository.SaveAllAsync())
            {
                return CreatedAtAction(nameof(GetMemberByUsername), new
                {
                    username = user.UserName
                }, _mapper.Map<PhotoDTO>(photo));
            }

            return BadRequest("Error adding photo");

        }

        [HttpPut("set-main-photo/{photoId}")]

        public async Task<ActionResult> setMainPhoto(int photoId)
        {
            var user = await this._userRepository.GetUserByUsernameAsync(User.getUsername());
            if (user == null) return NotFound();

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if(photo == null) return NotFound();
            if(photo.isMain) return BadRequest("This already your main photo");

            var currentMainPhoto = user.Photos.FirstOrDefault(x => x.isMain);
            if (currentMainPhoto != null) currentMainPhoto.isMain = false;
            photo.isMain = true;

            if(await _userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Error setting the main photo");
        }
    }

}

