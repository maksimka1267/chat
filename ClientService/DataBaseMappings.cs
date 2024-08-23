
using AutoMapper;
using ClientService.Entity;
using ClientService.Model;

namespace ClientService
{
    public class DataBaseMappings : Profile
    {
        public DataBaseMappings()
        {
            CreateMap<ClientEntity, Client>()
            .ConstructUsing(src => Client.Create(src.userName, src.password, src.email))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.friends, opt => opt.MapFrom(src => src.friends ?? new List<Guid>()));

            CreateMap<Client, ClientEntity>();
        }
    }
}
