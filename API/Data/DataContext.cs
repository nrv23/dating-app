
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : IdentityDbContext<AppUser, AppRole, int,
        IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>> // indica que el identificador sea de tipo entero para roles y usuarios
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }

        // agregar las entidades que van a ser las tablas de la bd 
        // public DbSet<AppUser> Users { get; set; } IdentityDbContext contiene una instancia , que se hereda en el datacontext
        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Connection> Connections { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) // usar este metodo para configurar relaciones de muchos a muchos.
        {
            base.OnModelCreating(builder);

            // configurar la relacion de muchos a muhcos entre usuario y role

            builder.Entity<AppUser>()
                .HasMany(r => r.UserRoles)
                .WithOne(u => u.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();


            builder.Entity<AppRole>()
                .HasMany(r => r.UserRoles)
                .WithOne(u => u.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

            builder.Entity<UserLike>() // asigna las llaves primarias de la relacion muchos a muchos
                .HasKey(k => new { k.SourceUserId, k.TargetUserId });

            builder.Entity<UserLike>()
                .HasOne(s => s.SourceUser)
                .WithMany(l => l.LikedUsers)
                .HasForeignKey(s => s.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserLike>()
                .HasOne(s => s.TargetUser)
                .WithMany(l => l.LikedbyUsers)
                .HasForeignKey(f => f.TargetUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Message>()
                .HasOne(u => u.Recipient)
                .WithMany(u => u.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict); // no va permitir borrar los mensajes aunque la cuentas e haya borrado

            builder.Entity<Message>()
                .HasOne(u => u.Sender)
                .WithMany(u => u.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict); // no va permitir borrar los mensajes aunque la cuentas e haya borrado
        }
    }
}