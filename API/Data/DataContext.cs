
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }

        // agregar las entidades que van a ser las tablas de la bd 

        public DbSet<AppUser> Users { get; set; }
        public DbSet<UserLike> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) // usar este metodo para configurar relaciones de muchos a muchos.
        {
            base.OnModelCreating(builder);
            builder.Entity<UserLike>() // asigna las llaves primarias de la relacion muchos a muchos
                .HasKey( k => new { k.SourceUserId,k.TargetUserId});

            builder.Entity<UserLike>()
                .HasOne(s => s.SourceUser)
                .WithMany(l => l.LikedUsers)
                .HasForeignKey(s => s.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserLike>()
                .HasOne(s => s.TargetUser)
                .WithMany(l => l.LikedbyUsers)
                .HasForeignKey( f => f.TargetUserId )
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}