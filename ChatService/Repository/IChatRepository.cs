using ChatService.Entity;

namespace ChatService.Repository
{
    public interface IChatRepository
    {
        Task<ChatEntity> CreateChatAsync(ChatEntity chat);
        Task<ChatEntity> GetChatByIdAsync(Guid id);
        Task<IEnumerable<ChatEntity>> GetAllChatsAsync();
        Task<IEnumerable<ChatEntity>> GetAllChatsofClientAsync(Guid clientId);
        Task UpdateChatAsync(ChatEntity chat);
        Task DeleteChatAsync(Guid id);
    }
}
