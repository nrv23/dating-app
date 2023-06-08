namespace API.Entities
{
    public class Photo
    {

        public int Id {get;set;}
        public string Url {get;set;}
        public bool isMain {get;set;} // indica si esta foto es la que se usa como foto de perfil
        public string PublicId {get;set;}
    }
}