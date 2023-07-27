using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.interfaces;
using API.DTOs;
using AutoMapper;
using API.Extensions;
using API.Entities;
using API.Helpers;

namespace API.Controllers
{
    [Authorize] // una vez que en el archivo program.cs se configura el middleware para autenticacion y autorizacion, se debe poner esta
    // anotacion en cada clase donde los servicios sean privados, en el nivel de mayor jerarquia para indicar que
    // todos los servicios de la clase son privados
    public class UsersController : BaseApiController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;


        public UsersController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
        {
          
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
            _photoService = photoService;
        }

       // [Authorize(Roles ="Member")] 
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDTO>> GetMemberByUsername(string username)
        {
            return await unitOfWork.UserRespository.GetMemberByUsernameAsync(username);
        }

       // [Authorize(Roles ="Admin")] // indicar el tipo de role de usuario que va tener acceso a este servicio
        [HttpGet]
        //[FromQuery] este indicador lo que esta diciendo es que los parametros vienen de la url 
        // algo como ?param1=1&param2=2
        public async Task<ActionResult<PagedList<MemberDTO>>> GetMembersAsync([FromQuery]UserParams userParams)
        {

            var Gender = await unitOfWork.UserRespository.GetUserGender(User.getUsername());
            userParams.CurrentUser = User.getUsername();

            if(string.IsNullOrEmpty(userParams.Gender)) {
                userParams.Gender = Gender == "male"? "female": "male";
            }

            var users = await unitOfWork.UserRespository.GetMembersAsync(userParams);
            // agregar la cabecera de pagination
            Response.AddPaginationHeader(new PaginationHeader(users.currentPage, users.PageSize,users.TotalCount, users.TotalPages));
            return Ok(users);
        }

        [HttpPut]
        public async Task<ActionResult> updateUser(memberUpdateDto memberUpdateDto)
        {

            var user = await unitOfWork.UserRespository.GetUserByUsernameAsync(User.getUsername());

            if (user == null) return NotFound();

            _mapper.Map(memberUpdateDto, user); // Lo que hace el mapper en este caso es que sobreescribe los valores de las propiedades 
            // del usuario con los datos que trae el data que es del tipo memberUpdateDto, actualiza las propiedades 
            // que comparten
            var updated = await unitOfWork.Completed();

            if (updated) return NoContent();

            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]

        public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
        {

            var user = await unitOfWork.UserRespository.GetUserByUsernameAsync(User.getUsername());
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

            if (await unitOfWork.Completed())
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
            var user = await unitOfWork.UserRespository.GetUserByUsernameAsync(User.getUsername());
            if (user == null) return NotFound();

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if(photo == null) return NotFound();
            if(photo.isMain) return BadRequest("This already your main photo");

            var currentMainPhoto = user.Photos.FirstOrDefault(x => x.isMain);
            if (currentMainPhoto != null) currentMainPhoto.isMain = false;
            photo.isMain = true;

            if(await unitOfWork.Completed()) return NoContent();
            return BadRequest("Error setting the main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> delete (int photoId) {

            var user = await unitOfWork.UserRespository.GetUserByUsernameAsync(User.getUsername());
            if (user == null) return NotFound();

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if(photo == null) return NotFound();
            if(photo.isMain) return BadRequest("You can not delete your main photo");

            var result = await this._photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);

            user.Photos.Remove(photo);

            if(await unitOfWork.Completed()) return Ok();
            return BadRequest("Error deleting the main photo");

        }
    }

}

