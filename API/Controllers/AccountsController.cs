using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.interfaces;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace API.Controllers
{
    [AllowAnonymous] // estos endpoints son publicos
    public class AccountsController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        public AccountsController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper)
        {
            this._mapper = mapper;
            _userManager = userManager;
            this._tokenService = tokenService;
        }

        [HttpPost("register")]

        public async Task<ActionResult<UserDTO>> register(RegisterDTO register)
        {

            if (await UserExists(register.username)) return BadRequest(new { msg = String.Concat(register.username, " ", "ya esta en uso") });

            var user = this._mapper.Map<AppUser>(register); // convierte automaticamente en una instancia de AppUser
            user.UserName = register.username.ToLower();

            var result = await _userManager.CreateAsync(user, register.password);
            var roleResult = await _userManager.AddToRoleAsync(user, "Member");

            if (!roleResult.Succeeded || !result.Succeeded) return BadRequest(result.Errors);

            return new UserDTO
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        [HttpPost("login")]

        public async Task<ActionResult<UserDTO>> login(LoginDTO data)
        {
            var user = await _userManager.Users
                .Include(x => x.Photos) // incluir la entidad relacionada para traer las fotos asociadas a ese usuario.
                .SingleOrDefaultAsync(x => x.UserName == data.username.ToLower());
            //SingleOrDefaultAsync este metodo debe devolver siempre una fila filtrando por un campo unico. SI devuelve mas de una, lanza una excepcion
            if (user == null) return BadRequest(new
            {
                msg = "Credenciales incorrectos"
            });

            var result = await _userManager.CheckPasswordAsync(user, data.password);

            if (!result) return Unauthorized(new
            {
                msg = "Credenciales incorrectos"
            });

            return new UserDTO
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.isMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower()); // devuvle un booleano
            // true si existe, false en el caso contrario
        }
    }
}