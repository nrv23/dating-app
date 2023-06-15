namespace API.DTOs
{
    public class memberUpdateDto
    {
        public string Introduction { get; set; }
        public string LookingFor { get; set; } // descripcion del perfil que busca
        public string Interests { get; set; } // descripcion del perfil que busca
        public string City { get; set; }
        public string Country { get; set; }

    }
}