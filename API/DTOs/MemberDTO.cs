
namespace API.DTOs
{
    public class MemberDTO
    {

        public int Id { get; set; }
        public string UserName { get; set; } // poner como disable la opcion <Nullable>disable</Nullable>  del archivo csproj para evitar el warning de que un sttring no puede ser nulo.
        public string PhotoUrl { get; set; }
        public int DateOfBirth { get; set; } //Va ser la edad 
        public string KnownAs { get; set; } // conocido como
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; } // descripcion del perfil que busca
        public string Interests { get; set; } // descripcion del perfil que busca
        public string City { get; set; }
        public string Country { get; set; }
        public List<PhotoDTO> Photos { get; set; } = new(); // declara la lista como vacía
    }
}