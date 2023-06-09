using API.Data;
using API.Entities;
using API.interfaces;
using Microsoft.EntityFrameworkCore;
namespace API.Repository
{
    public class UserRepository : IUserRespository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context) {
            _context = context;
        }
        public async Task<AppUser> GetUserByIdAsync(int userId)
        {
            return await _context
                .Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync( x => x.Id == userId);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context
                .Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync( x => x.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context
                .Users
                .Include(p => p.Photos)
                .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0; // esto indica si las transacciones se ejecutaron bine.
        }

        public void Update(AppUser user)
        {
           _context.Entry(user).State = EntityState.Modified; // se usa para indicar que una entidad a tenido una actualizacion
        }
    }
}