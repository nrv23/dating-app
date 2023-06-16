using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.interfaces;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [AllowAnonymous] // estos endpoints son publicos
    public class AccountsController : BaseApiController
    {
        private readonly DataContext context;
        private readonly ITokenService _tokenService;
        public AccountsController(DataContext _context, ITokenService tokenService)
        {
            this.context = _context;
            this._tokenService = tokenService;
        }

        [HttpPost("register")]

        public async Task<ActionResult<UserDTO>> register(RegisterDTO register)
        {

            if (await UserExists(register.username))
            {

                return BadRequest(new
                {
                    msg = String.Concat(register.username, " ", "ya esta en uso")
                });
            }
            using var hmac = new HMACSHA512();
            // la palabra reservada using lo que hace es que una vez que la instancia de la clase HMACSHA512 no se use mas,
            // va borrarla de la memoria temporal
            var user = new AppUser
            {
                UserName = register.username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(register.password)),
                PasswordSalt = hmac.Key // esto lo genera de forma ranmdon
            };

            context.Users.Add(user);

            await context.SaveChangesAsync();

            return new UserDTO
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]

        public async Task<ActionResult<UserDTO>> login(LoginDTO data)
        {
            var user = await context.Users
                .Include(x => x.Photos) // incluir la entidad relacionada para traer las fotos asociadas a ese usuario.
                .SingleOrDefaultAsync(x => x.UserName == data.username.ToLower());
            //SingleOrDefaultAsync este metodo debe devolver siempre una fila filtrando por un campo unico. SI devuelve mas de una, lanza una excepcion
            if (user == null) return BadRequest(new
            {
                msg = "Credenciales incorrectos"
            });

            // usar el salt del usuario para generar el password hash
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data.password));
            // comparar las cadenas de las contraseñas hasheadas.

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized(new
                {
                    msg = "Credenciales incorrectos"
                });
            }

            return new UserDTO
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.isMain)?.Url
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await context.Users.AnyAsync(x => x.UserName == username.ToLower()); // devuvle un booleano
            // true si existe, false en el caso contrario
        }
    }
}