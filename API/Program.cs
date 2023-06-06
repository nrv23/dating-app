using Microsoft.EntityFrameworkCore;
using API.Data;
using API.interfaces;
using API.services;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using API.Extensions;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddAplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


// el builder es un contenedor de servicios donde se asigna cualquier servicio que la aplicacion va tener
var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);

// usar los middlewares para autenticacion y autorizacion
app.UseAuthentication();
app.UseAuthorization();
// Configure the HTTP request pipeline.
app.MapControllers(); // este metodo es el que maneja las peticiones entrantes y llama los controladores que correspondan a cada 
// endpoint

app.Run();
