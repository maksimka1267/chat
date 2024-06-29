using ClientService.Model;

namespace ClientService.Repository
{
    public interface IClientRepository
    {
        Task<Client> GetClientByEmail(string email);

        Task<Client> GetClientById(Guid id);
        Task<Client> GetClientByUserName(string userName);

        Task<bool> AddClient(Client client);
        Task<bool> AddSession(Guid clientId, Guid sessionId);
    }
}
