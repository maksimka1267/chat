using APIGateway.Model;

namespace APIGateway.Interface
{
    public interface IClientRepository
    {
        Task<bool> RegisterClient(string username, string password, string email);
        Task<bool> DeleteAccount(Guid clientId);
        Task<bool> AddFriend(Guid clientId, Guid friendsId);
        Task<bool> DeleteFriend(Guid clientId, Guid friendsId);
        Task<ClientDto> GetPersonalCabinet(Guid id);
        Task<string> Login(string email, string password);
        Task<Client> GetClientInfo(Guid clientId);
        Task<List<Client>> GetListFriends(Guid clientId);
    }
}
