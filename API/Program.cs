using API.Data;
using API.Entities;
using API.Extensions;
using API.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddAplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


// el builder es un contenedor de servicios donde se asigna cualquier servicio que la aplicacion va tener
var app = builder.Build();

// agregar el middleware para manejo global de exepciones

app.UseMiddleware<ExceptionMiddleware>(); // manejar excepciones globalmente
app.UseCors(MyAllowSpecificOrigins);

// usar los middlewares para autenticacion y autorizacion
app.UseAuthentication();
app.UseAuthorization();
// Configure the HTTP request pipeline.
app.MapControllers(); // este metodo es el que maneja las peticiones entrantes y llama los controladores que correspondan a cada 
// endpoint

// llamar metodo para generar el seed de usuarios

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();
    var userMansger = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    
    await context.Database.MigrateAsync();
    await Seed.SeedUsers(userMansger,roleManager);
}
catch (Exception ex) 
{
    var logger = services.GetService<ILogger<Program>>();
    logger.LogError(ex, "Hubo un error en la migracion");
}
app.Run();
