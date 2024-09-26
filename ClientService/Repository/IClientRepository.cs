using ClientService.Model;

namespace ClientService.Repository
{
    public interface IClientRepository
    {
        Task<Client> GetClientByEmail(string email);

        Task<Client> GetClientById(Guid id);
        Task<Client> GetClientByUserName(string userName);
        Task<bool> RegisterClientAsync(Client client);
        Task<bool> AddClient(Client client);
        Task<bool> AddFriend(Guid clientId, Guid friendsId);
        Task AddChatToClientAsync(Guid chatId, Guid clientId);
        Task<bool> DeleteFriend(Guid clientId, Guid friendsId);
        Task<bool> DeleteClient(Guid client);
        Task<List<Client>> GetListFriends(Guid client);
    }
}
