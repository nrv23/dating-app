
using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles: Profile
    {   
        public AutoMapperProfiles() {
            CreateMap<AppUser,MemberDTO>()
            // cuando una propieda del DTO es de diferente tipo de dato, se tiene que setear con la informacion del mismo tipo
            /*
                En este caso el campo DateOfBirth de dto es int y el dateOfBirth del AppUser es Date, no hay conversion implicata y por eso
                se usa el forMember que hace referencia a la propiedad del DTo y map.from hace referencia al dato de la entiedad que va setear la propiedad
                del DTO
            */
                .ForMember(des => des.DateOfBirth, opt => opt.MapFrom(src => src.getAge() ))
                .ForMember(des => des.PhotoUrl,
                     opt => opt.MapFrom(src => 
                        src.Photos.FirstOrDefault(x => x.isMain).Url)); // como Photos es una entidad, debe recorrer esa entidad y fitrar por el isMain
                        // luego obtiene la propiedad Url
            CreateMap<Photo,PhotoDTO>();
        }
    }   
}