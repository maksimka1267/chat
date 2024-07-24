using MessageService.Model;

namespace MessageService.Repository
{
    public interface IMessageRepository
    {
        Task<bool> SendMessageAsync(Message message);
        Task<Message> GetMessageAsync(Guid messageId);
        Task<bool> DeleteMessageAsync(Guid messageId);
        Task<bool> UpdateMessageAsync(Guid id, string message);
        Task<IEnumerable<Message>> FindMessagesAsync(string text);
    }
}
