

namespace API.Entities
{
    public class AppUser
    {
        public int Id { get; set; } 
        public string UserName { get; set; } // poner como disable la opcion <Nullable>disable</Nullable>  del archivo csproj para evitar el warning de que un sttring no puede ser nulo.
    }
}