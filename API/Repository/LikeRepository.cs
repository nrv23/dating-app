using API.Data;
using API.DTOs;
using API.Entities;
using API.interfaces;
using API.Extensions;
using Microsoft.EntityFrameworkCore;
using API.Helpers;

namespace API.Repository
{
    public class LikeRepository : ILikeRepository
    {
        private readonly DataContext _context;
        
        public LikeRepository(DataContext context)
        {
            _context = context;
            
        }
        public async Task<UserLike> GetUserLike(int sourceUserId, int tagretUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, tagretUserId);
        }

        public async Task<PagedList<LikeDTO>> GetUserLikes(LikesParams likesParams)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            if (likesParams.Predicate == "liked") // los likes del usuario actual
            {
                likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
                users = likes.Select(like => like.TargetUser); // hace la conexion con la entidnad usuario para traer la informacion
            }

            if (likesParams.Predicate == "likedBy") // los likes de un usuario consultado
            {
                likes = likes.Where(like => like.TargetUserId == likesParams.UserId);
                users = likes.Select(like => like.SourceUser); // hace la conexion con la entidnad usuario para traer la informacion
            }

            var likedUsers =  users.Select(user => new LikeDTO
            {
                UserName = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.isMain).Url,
                City = user.City,
                Id = user.Id
            });

            return await PagedList<LikeDTO>.CreateAsync(likedUsers,likesParams.PageNumber,likesParams.PageSize);
        }

      

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users
                .Include(x => x.LikedUsers) // incluye los likes del usaurio
                .FirstOrDefaultAsync( x => x.Id == userId);
        }
    }
}