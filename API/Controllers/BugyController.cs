

using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
namespace API.Controllers
{
    public class BugyController : BaseApiController
    {
        private readonly DataContext _context;

        public BugyController(DataContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> getSecrets()
        {

            return "secret text";
        }

        [HttpGet("not-found")]
        public IActionResult getNotFound()
        {
            var thing = _context.Users.Find(-1);
            return thing == null ? NotFound(new{
                msg = "not found"
            }) : Ok(thing);
        }


        [HttpGet("server-error")]
        public ActionResult<string> getServerError()
        {


            var thing = _context.Users.Find(-1);
            return thing.ToString();



        }

        [HttpGet("bad-request")]
        public ActionResult<string> getBadRequest()
        {

            return BadRequest("This was a  bad request");
        }
    }
}