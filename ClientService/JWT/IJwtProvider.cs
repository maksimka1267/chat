using ClientService.Model;

namespace ClientService.JWT
{
    public interface IJwtProvider
    {
        string GenerateToken(Client client);
    }
}
