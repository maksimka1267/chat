namespace APIGateway.Interface
{
    public interface IChatRepository
    {
        Task<bool> RegisterClient(string username, string password, string email);

        Task AddChat(Guid sessionId, Guid clientId);
        //Task<ClientDto> GetPersonalCabinet(Guid id);
        Task<string> Login(string email, string password);
    }
}
