using APIGateway.Model;

namespace APIGateway.Interface
{
    public interface IClientRepository
    {
        Task<bool> RegisterClient(string username, string password, string email);

        Task<bool> AddChat(Guid chatnId, Guid clientId);
        Task<bool> AddFriend(Guid friendId, Guid clientId);
        Task<ClientDto> GetPersonalCabinet(Guid id);
        Task<string> Login(string email, string password);
    }
}
