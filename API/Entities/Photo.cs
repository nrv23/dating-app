using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    [Table("Photos")] // se le da el nombre de la tabla sino se quiere que tome el nombre de la clase
    public class Photo
    {

        public int Id {get;set;}
        public string Url {get;set;}
        public bool isMain {get;set;} // indica si esta foto es la que se usa como foto de perfil
        public string PublicId {get;set;}
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; } // esto genera la llae foranea con Usuarios y no permite que sea nula
    }
}