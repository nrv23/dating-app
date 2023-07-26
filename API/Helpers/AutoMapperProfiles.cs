
using API.DTOs;
using API.Entities;
using AutoMapper;
using API.Extensions;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDTO>()
            // cuando una propieda del DTO es de diferente tipo de dato, se tiene que setear con la informacion del mismo tipo
            /*
                En este caso el campo DateOfBirth de dto es int y el dateOfBirth del AppUser es Date, no hay conversion implicata y por eso
                se usa el forMember que hace referencia a la propiedad del DTo y map.from hace referencia al dato de la entiedad que va setear la propiedad
                del DTO
            */
                .ForMember(des => des.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()))
                .ForMember(des => des.PhotoUrl,
                     opt => opt.MapFrom(src =>
                        src.Photos.FirstOrDefault(x => x.isMain).Url)); // como Photos es una entidad, debe recorrer esa entidad y fitrar por el isMain
                                                                        // luego obtiene la propiedad Url
            CreateMap<Photo, PhotoDTO>();
            CreateMap<memberUpdateDto, AppUser>(); //los parametros son source y destination

            /*
                cuando es un get siempre se debe poner como source la entidad y destination el dto
                cuando es un insert, update se debe poner como sooruce el dto y destino la entidad.
            */
            CreateMap<RegisterDTO, AppUser>(); // se crea el mapeo para registrar un nuevo usuario
            CreateMap<Message, MessageDTO>()
               .ForMember(dest => dest.SenderPhotoUrl, opt => opt.MapFrom(src =>
                   src.Sender.Photos.FirstOrDefault(x => x.isMain).Url))
               .ForMember(dest => dest.RecipientPhotoUrl, opt => opt.MapFrom(src =>
                   src.Recipient.Photos.FirstOrDefault(x => x.isMain).Url));
                   // convertir fechas a fechas con utc 

            CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
            CreateMap<DateTime?, DateTime?>().ConvertUsing(d => d.HasValue ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : null);
        }
    }
}