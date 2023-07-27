using API.interfaces;
using Microsoft.AspNetCore.Mvc;
using API.Extensions;
using API.Entities;
using API.DTOs;
using API.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {

        private readonly IUnitOfWork UnitOfWork;

        public LikesController(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
           
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = int.Parse(User.getUserId());
            var likedUser = await UnitOfWork.UserRespository.GetUserByUsernameAsync(username);
            var sourceUser = await UnitOfWork.LikeRepository.GetUserWithLikes(sourceUserId);

            if (likedUser == null) return NotFound();
            if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");

            var userLike = await UnitOfWork.LikeRepository.GetUserLike(sourceUserId, likedUser.Id);

            if (userLike != null) return BadRequest("You already like this user");
            userLike = new UserLike
            {
                SourceUserId = sourceUserId,// usuario conectado
                TargetUserId = likedUser.Id // se le da el like al usuario
            };

            sourceUser.LikedUsers.Add(userLike);

            if (await UnitOfWork.Completed()) return Ok();
            return BadRequest("Faile to like user");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDTO>>> GetUserLikes([FromQuery] LikesParams likesParams)
        {
            likesParams.UserId = int.Parse(User.getUserId());
            var users = await UnitOfWork.LikeRepository.GetUserLikes(likesParams);
              Response.AddPaginationHeader(new PaginationHeader(users.currentPage, users.PageSize,users.TotalCount, users.TotalPages));
            return Ok(users);
        }
    }
}