

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userMansger, RoleManager<AppRole> roleManager)
        {

            if (await userMansger.Users.AnyAsync()) return; // valida si hay usuarios creados de prueba
            // ReadAllTextAsync, tomo base la ruta root del proyecto que seria .API/API/

            var usersData = await File.ReadAllTextAsync("Data/UserSeedData.json");
            // evitar errores con el nombre de las propiedades 

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var users = JsonSerializer.Deserialize<List<AppUser>>(usersData);
            var roles = new List<AppRole>{
                new AppRole{Name ="Member"},
                new AppRole{Name ="Admin"},
                new AppRole{Name ="Moderator"},
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            foreach (var user in users) // generar contrasenas 
            {
                user.UserName = user.UserName.ToLower();
                await userMansger.CreateAsync(user, "Pa$$w0rd");// agrega al contexto de usuario 
                await userMansger.AddToRoleAsync(user, "Member");
            }

            // crear usuario admin
            var admin = new AppUser
            {
                UserName = "Admin"
            };

            await userMansger.CreateAsync(admin, "Pa$$w0rd");// agrega al contexto de usuario 
            await userMansger.AddToRolesAsync(admin, new[]{"Admin","Moderator" });
        }
    }
}