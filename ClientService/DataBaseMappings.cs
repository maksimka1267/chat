using AutoMapper;
using ClientService.Model;
using ClientService;
using ClientService.Entity;

namespace ClientService
{
    public class DataBaseMappings : Profile
    {
        public DataBaseMappings()
        {
            // Маппинг между ClientEntity и Client
            CreateMap<ClientEntity, Client>()
                .ConstructUsing(src => Client.Create(src.userName, src.password, src.email))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.friends, opt => opt.MapFrom(src => src.friends ?? new List<Guid>()));

            CreateMap<Client, ClientEntity>();

            // Добавьте маппинг между Client и ClientMessage
            CreateMap<Client, ClientMessage>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.userName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.email))
                .ForMember(dest => dest.FriendsIds, opt => opt.MapFrom(src => src.friends.Select(f => f.ToString()).ToArray()));
        }
    }
}
