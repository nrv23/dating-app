

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext context){

            if(await context.Users.AnyAsync()) return;
            // ReadAllTextAsync, tomo base la ruta root del proyecto que seria .API/API/

            var usersData = await File.ReadAllTextAsync("Data/UserSeedData.json");
            // evitar errores con el nombre de las propiedades 

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var users = JsonSerializer.Deserialize<List<AppUser>>(usersData);
            foreach (var user in users) // generar contrasenas 
            {
                using var hmac = new HMACSHA512();

                user.UserName = user.UserName.ToLower();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
                user.PasswordSalt = hmac.Key; // se genera random

                context.Users.Add(user); // agrega al contexto de usuario 
            }

            await context.SaveChangesAsync(); // guarda en base de datos
        }
    }
}