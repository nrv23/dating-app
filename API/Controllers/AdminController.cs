using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AdminController: BaseApiController
    {
        [Authorize(Policy = "RequireAdminRole" )]
        [HttpGet("users-with-roles")]
        public ActionResult GetUsersWithRoles(){

            return Ok("Only can see this");
        }

        [Authorize(Policy = "ModeratePhotoRole" )]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotoForModeration(){
            return Ok("Admin o moderator can see this");
        }
    }
}