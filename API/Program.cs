using Microsoft.EntityFrameworkCore;
using API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// agregar el context para cargar el servicio de conexion con la bd 
builder.Services.AddDbContext<DataContext>(opt => {
    // se configura la conexion de sqlite
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// el builder es un contenedor de servicios donde se asigna cualquier servicio que la aplicacion va tener
var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapControllers(); // este metodo es el que maneja las peticiones entrantes y llama los controladores que correspondan a cada 
// endpoint

app.Run();
