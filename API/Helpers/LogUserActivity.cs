
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
            var unitOfWork = resultContext.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
            var user = await unitOfWork.UserRespository.GetUserByIdAsync( int.Parse(userId));
            user.LastActive = DateTime.UtcNow; // actualizar el campo lastActive

            await unitOfWork.Completed();
        }
    }
}