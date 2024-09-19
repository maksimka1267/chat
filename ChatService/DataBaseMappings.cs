using AutoMapper;
using ChatService.Entity;
using ChatService.Model;

namespace ChatService
{
    public class DataBaseMappings : Profile
    {
        public DataBaseMappings()
        {
            CreateMap<ChatEntity, Chat>().ReverseMap();
        }
    }
}
