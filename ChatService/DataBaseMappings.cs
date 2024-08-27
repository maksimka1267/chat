using AutoMapper;
using ChatService.Entity;
using ChatService.Model;

namespace ChatService
{
    public class DataBaseMappings : Profile
    {
        public DataBaseMappings()
        {
            CreateMap<ChatEntity, Chat>()
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Chaters, opt => opt.MapFrom(src => src.Chaters))
                .ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.Messages))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));
        }
    }
}
