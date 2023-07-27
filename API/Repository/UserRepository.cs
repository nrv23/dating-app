using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
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

        public async Task<PagedList<MemberDTO>> GetMembersAsync(UserParams userParams)
        {
            var query = _context.Users.AsQueryable();
            query = query.Where(x => x.UserName != userParams.CurrentUser);
            query = query.Where(x => x.Gender != userParams.Gender);

            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge - 1));

            Console.WriteLine("edad", maxDob);
            Console.WriteLine("tanaño de pagina".Concat(userParams.PageSize.ToString()));

            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            // ordenar la consulta

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(x => x.Created),
                _ => query.OrderByDescending(u => u.LastActive) // ordernamiento por default
            };

            return await PagedList<MemberDTO>.CreateAsync(
                    query.AsNoTracking().ProjectTo<MemberDTO>(_mapper.ConfigurationProvider),
                    userParams.PageNumber,
                    userParams.PageSize
                );
        }
        // //////////////////////////////////
        public async Task<AppUser> GetUserByIdAsync(int userId)
        {
            return await _context
                .Users
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
        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified; // se usa para indicar que una entidad a tenido una actualizacion
        }

        public async Task<string> GetUserGender(string username)
        {
            return await _context.Users.Where(x => x.UserName == username)
            .Select(x => x.Gender).FirstOrDefaultAsync(); // selecciona solo el genero
        }
    }
}