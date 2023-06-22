
using Microsoft.AspNetCore.Mvc.Filters;
using API.Extensions;
using API.interfaces;
namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next(); // ejecuta el siguiente middleware y traer la respuesta

            // validar que esta autenticado 
            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            var userId = resultContext.HttpContext.User.getUserId();
            var repo = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRespository>();
            var user = await repo.GetUserByIdAsync( int.Parse(userId));
            user.LastActive = DateTime.UtcNow; // actualizar el campo lastActive

            await repo.SaveAllAsync();
        }
    }
}