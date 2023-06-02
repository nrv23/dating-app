using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // se puede dejar como esta para que tome el nombre de controlador por defecto o asignarle un nombre personalizado


    public class UsersController: ControllerBase
    {
        private readonly DataContext context;

        public UsersController(DataContext context)
        {
            this.context = context;

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            var users = await this.context.Users.ToListAsync();
            return users;
        }

        [HttpGet("{id}")]

        public async Task<ActionResult<AppUser>> GetById(int id) {

            var user = await context.Users.FindAsync(id);
            //el metodo find o findAsync busca por la llave primaria de la entiendad.
            return user;
        }
    }
}