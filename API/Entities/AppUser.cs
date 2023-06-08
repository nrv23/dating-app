

namespace API.Entities
{
    public class AppUser
    {
        public int Id { get; set; } 
        public string UserName { get; set; } // poner como disable la opcion <Nullable>disable</Nullable>  del archivo csproj para evitar el warning de que un sttring no puede ser nulo.
        public byte[] PasswordHash {get;set;}
        public byte[] PasswordSalt {get;set;}
        public DateOnly DateOfBirth{get;set;} //DateOnly en sql seria tipo date
        public string KnownAs {get;set;} // conocido como
        public DateTime Created {get;set;} = DateTime.UtcNow;
        public DateTime LastActive {get;set;} = DateTime.UtcNow; // ultia vez que hizo sesion
        public string Gender {get;set;}
        public string Introduction {get;set;}
        public string LookingFor {get;set;} // descripcion del perfil que busca
        public string Interests {get;set;} // descripcion del perfil que busca
        public  string City {get;set;} 
        public  string Country {get;set;} 
        public List<Photo> Photos {get;set;} = new(); // declara la lista como vacía

    }
}