
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {

            // agregar validacion de token para que el servidor sepa si hay una sesion valida o no
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true, // si la firma es correcta
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
                    // limitar el envio de tokens para ciertos criterios como las ips, para limitar el acceso de la aplicacion
                    // por ahora la aplicacion sera global
                    ValidateAudience = false,
                    ValidateIssuer = false
                };
            });

            return services;
        }
    }
}