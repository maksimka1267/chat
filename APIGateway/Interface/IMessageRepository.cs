using APIGateway.Model;
using ServiceMessage;

namespace APIGateway.Interface
{
    public interface IMessageRepository
    {
        Task<MessageModel> SendMessage(Guid author, Guid chatId, string? text, byte[]? photo);
        Task<MessageModel> GetMessage(Guid id);
        Task<MessageModel> UpdateMessage(Guid id, string text);
        Task<bool> DeleteMessage(Guid id);
    }
}
