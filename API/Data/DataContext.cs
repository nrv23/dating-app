
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions options): base(options) { 

        }

        // agregar las entidades que van a ser las tablas de la bd 

        public DbSet<AppUser> Users {get;set;}
    }
}