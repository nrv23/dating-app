
using API.interfaces;
using API.Repository;
using API.services;
using Microsoft.EntityFrameworkCore;
using API.Helpers;


namespace API.Extensions
{
    public static class AplicationServiceExtensions
    {
        public static IServiceCollection AddAplicationServices(this IServiceCollection services, IConfiguration config)
        {

            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
            // add cors 
            services.AddCors(options =>
            {
                options
                    .AddPolicy(name: MyAllowSpecificOrigins,
                        policy => policy
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .WithOrigins("https://localhost:4200")
                    );
            });

            // se agrega un servicio para manejo de token
            services.AddScoped<ITokenService, TokenService>();

            services.AddScoped<IUserRespository, UserRepository>(); // se debe agregar para que .net cree una instancia
            // habilitar el automapper 
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //agregar la configuracion de cloudinary
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            // agregar el servicio que subir las fotos a cloudinary
            services.AddScoped<IPhotoService, PhotoService>();
            // servicio de fotos
            services.AddScoped<ILikeRepository, LikeRepository>();
            // servicio de mensajes
            services.AddScoped<IMessageRepository, MessageRepository>();

            services.AddScoped<LogUserActivity>();
            // agregar el context para cargar el servicio de conexion con la bd 
            services.AddDbContext<Data.DataContext>(opt =>
            {
                // se configura la conexion de sqlite
                opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

            //services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

            return services;
        }
    }
}