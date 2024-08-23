using AutoMapper;
using MessageService.Entity;
using MessageService.Model;

namespace MessageService
{
    public class DataBaseMappings : Profile
    {
        public DataBaseMappings()
        {
            CreateMap<MessageEntity, Message>()
                .ConstructUsing(src => Message.Create(src.Text, src.Author, src.Photo))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date));
            CreateMap<Message, MessageEntity>();
        }
    }
}
