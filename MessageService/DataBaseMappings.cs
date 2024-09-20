using AutoMapper;
using MessageService.Entity;
using ServiceMessage;

namespace MessageService
{
    public class DataBaseMappings : Profile
    {
        public DataBaseMappings()
        {
            // Маппинг между Message и MessageEntity
            CreateMap<Message, MessageEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
                .ForMember(dest => dest.Chat, opt => opt.MapFrom(src => src.Chat))
                .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Photo));

            // Маппинг между Message и ServiceMessage.Message (для передачи через gRPC)
            CreateMap<Message, ServiceMessage.Message>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author.ToString()))
                .ForMember(dest => dest.ChatId, opt => opt.MapFrom(src => src.Chat.ToString()))
                .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Photo != null ?
                    Google.Protobuf.ByteString.CopyFrom(src.Photo) :
                    Google.Protobuf.ByteString.Empty)); // Обработка null значений

            // Маппинг из gRPC модели (ServiceMessage.Message) в доменную модель (Message)
            CreateMap<ServiceMessage.Message, Message>()
                .ConstructUsing(src => new Message
                {
                    Text = src.Text,
                    Author = Guid.Parse(src.Author),
                    Chat = Guid.Parse(src.ChatId),
                    Photo = src.Photo.ToByteArray()
                });

            // Обратный маппинг, если необходимо преобразовывать MessageEntity в Message
            CreateMap<MessageEntity, Message>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
                .ForMember(dest => dest.Chat, opt => opt.MapFrom(src => src.Chat))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Photo));

            // Новый маппинг между MessageEntity и ServiceMessage.Message
            CreateMap<MessageEntity, ServiceMessage.Message>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author.ToString()))
                .ForMember(dest => dest.ChatId, opt => opt.MapFrom(src => src.Chat.ToString()))
                .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Photo != null ?
                    Google.Protobuf.ByteString.CopyFrom(src.Photo) :
                    Google.Protobuf.ByteString.Empty));

            // Обратный маппинг из gRPC модели (ServiceMessage.Message) в MessageEntity
            CreateMap<ServiceMessage.Message, MessageEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => Guid.Parse(src.Author)))
                .ForMember(dest => dest.Chat, opt => opt.MapFrom(src => Guid.Parse(src.ChatId)))
                .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Photo.ToByteArray()));
        }
    }
}
