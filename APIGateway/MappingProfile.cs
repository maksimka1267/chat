using AutoMapper;
using APIGateway.Model;
using ClientService;
using ServiceMessage;
using ServiceChat;
using Google.Protobuf.WellKnownTypes;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Маппинг для ClientMessage и Client
        CreateMap<ClientMessage, Client>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.userName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.password, opt => opt.MapFrom(src => src.Password))
            .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.friends, opt => opt.MapFrom(src => src.FriendsIds != null ? src.FriendsIds.Select(f => f.ToString()).ToArray() : null))
            .ForMember(dest => dest.chats, opt => opt.MapFrom(src => src.ChatIds != null ? src.ChatIds.Select(c => c.ToString()).ToArray() : null));

        // Маппинг для GetClientInfoResponse и Client
        CreateMap<GetClientInfoResponse, Client>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Client.Id.ToString()))
            .ForMember(dest => dest.userName, opt => opt.MapFrom(src => src.Client.UserName))
            .ForMember(dest => dest.password, opt => opt.MapFrom(src => src.Client.Password))
            .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.Client.Email))
            .ForMember(dest => dest.friends, opt => opt.MapFrom(src => src.Client.FriendsIds != null ? src.Client.FriendsIds.Select(f => f.ToString()).ToArray() : null))
            .ForMember(dest => dest.chats, opt => opt.MapFrom(src => src.Client.ChatIds != null ? src.Client.ChatIds.Select(c => c.ToString()).ToArray() : null));

        // Маппинг для Message и MessageModel
        CreateMap<Message, MessageModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author.ToString()))
            .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Photo != null ? src.Photo : null));

        // Маппинг для Chat и ChatModel
        CreateMap<Chat, ChatModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Chaters, opt => opt.MapFrom(src => src.Chaters.Select(c => c.ToString()).ToArray()))
            .ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.Messages != null ? src.Messages.Select(m => m.ToString()).ToArray() : null))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToDateTime().ToString("o")))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt != null ? src.UpdatedAt.ToDateTime().ToString("o") : null));

        // Маппинг для GetMessageResponse и MessageModel
        CreateMap<GetMessageResponse, MessageModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Message.Id.ToString()))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Message.Text))
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Message.Author.ToString()))
            .ForMember(dest => dest.ChatId, opt => opt.MapFrom(src => src.Message.ChatId.ToString()))
            .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Message.Photo != null ? src.Message.Photo : null))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateTime.Parse(src.Message.Date)));
    }
}
