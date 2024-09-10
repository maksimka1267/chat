using APIGateway.Interface;

namespace APIGateway.Repository
{
    public class ChatRepository : IChatRepository
    {
        public Task AddChat(Guid sessionId, Guid clientId)
        {
            throw new NotImplementedException();
        }

        public Task<string> Login(string email, string password)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RegisterClient(string username, string password, string email)
        {
            throw new NotImplementedException();
        }
    }
}
