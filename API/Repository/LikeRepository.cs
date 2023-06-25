using API.Data;
using API.DTOs;
using API.Entities;
using API.interfaces;
using API.Extensions;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<LikeDTO>> GetUserLikes(string predicate, int userId)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            if (predicate == "liked") // los likes del usuario actual
            {
                likes = likes.Where(like => like.SourceUserId == userId);
                users = likes.Select(like => like.TargetUser); // hace la conexion con la entidnad usuario para traer la informacion
            }

            if (predicate == "likedBy") // los likes de un usuario consultado
            {
                likes = likes.Where(like => like.TargetUserId == userId);
                users = likes.Select(like => like.SourceUser); // hace la conexion con la entidnad usuario para traer la informacion
            }

            return  await users.Select(user => new LikeDTO
            {
                UserName = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.isMain).Url,
                City = user.City,
                Id = user.Id
            }).ToListAsync();
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users
                .Include(x => x.LikedUsers) // incluye los likes del usaurio
                .FirstOrDefaultAsync( x => x.Id == userId);
        }
    }
}