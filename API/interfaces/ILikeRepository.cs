using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.interfaces
{
    public interface ILikeRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int tagretUserId);
        Task<AppUser> GetUserWithLikes(int userId);
        Task<PagedList<LikeDTO>> GetUserLikes(LikesParams likesParams);
    }
} 