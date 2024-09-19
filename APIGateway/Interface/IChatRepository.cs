using APIGateway.Model;

namespace APIGateway.Interface
{
    public interface IChatRepository
    {
        public Task<ChatModel> CreateChat(string name, List<Guid> chaters);
        public Task<ChatModel> UpdateChat(Guid id, string name);
        public Task<ChatModel> DeleteChat(Guid chatId);
        public Task<ChatModel> GetChatById(Guid chatId);
        public Task<IEnumerable<ChatModel>> GetChatsByUserId(Guid userId);
    }
}
