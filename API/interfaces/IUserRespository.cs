

using API.Entities;

namespace API.interfaces
{
    public interface IUserRespository
    {
        void Update(AppUser user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByIdAsync(int userId);
        Task<AppUser> GetUserByUsernameAsync(string username);
    }
}