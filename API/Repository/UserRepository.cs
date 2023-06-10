using API.Data;
using API.DTOs;
using API.Entities;
using API.interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
namespace API.Repository
{
    public class UserRepository : IUserRespository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MemberDTO> GetMemberByUsernameAsync(string username)
        {
            return await _context.Users
                .Where(x => x.UserName == username)
                .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider) // mapea los datos de manera que la consulta solo devuelva lo que se necesita
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<MemberDTO>> GetMembersAsync()
        {
            return await _context.Users
                .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider) // mapea los datos de manera que la consulta solo devuelva lo que se necesita
                .ToListAsync();
        }


        // //////////////////////////////////
        public async Task<AppUser> GetUserByIdAsync(int userId)
        {
            return await _context
                .Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.Id == userId);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context
                .Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == username);
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