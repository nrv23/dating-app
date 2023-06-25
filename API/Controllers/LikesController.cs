using API.interfaces;
using Microsoft.AspNetCore.Mvc;
using API.Extensions;
using API.Entities;
using API.DTOs;

namespace API.Controllers
{
    public class LikesController : BaseApiController
    {
        private readonly IUserRespository _userRepository;
        private readonly ILikeRepository _likeRepository;

        public LikesController(IUserRespository userRepository, ILikeRepository likeRepository)
        {
            _userRepository = userRepository;
            _likeRepository = likeRepository;
        }

        [HttpPost()]

        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = int.Parse(User.getUserId());
            var likedUser = await _userRepository.GetUserByUsernameAsync(username);
            var sourceUser = await _likeRepository.GetUserWithLikes(sourceUserId);

            if (likedUser == null) return NotFound();

            if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");

            var userLike = await _likeRepository.GetUserLike(sourceUserId, likedUser.Id);

            if (userLike != null) return BadRequest("You already like this user");

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,// usuario conectado
                TargetUserId = likedUser.Id // se le da el like al usuario
            };

            sourceUser.LikedUsers.Add(userLike);

            if( await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Faile to like user");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDTO>>> GetUserLikes([FromQuery]string predicate) {
            var users = await _likeRepository.GetUserLikes(predicate, int.Parse(User.getUserId()));
            return Ok(users);
        }
    }
}