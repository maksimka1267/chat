using MessageService.Entity;

namespace MessageService.Repository
{
    public interface IMessageRepository
    {
        Task<MessageEntity> SendMessageAsync(MessageEntity message);
        Task<MessageEntity> GetMessageAsync(Guid messageId);
        Task DeleteMessageAsync(Guid messageId);
        Task UpdateMessageAsync(Guid id, string message);
        Task<IEnumerable<MessageEntity>> FindMessagesAsync(string text);
    }
}
