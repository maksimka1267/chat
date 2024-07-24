using AutoMapper;
using ClientService.Data;
using ClientService.Entity;
using ClientService.Model;
using Microsoft.EntityFrameworkCore;

namespace ClientService.Repository
{
    public class ClientRepository : IClientRepository
    {
        private readonly ClientDbContext _context;
        private readonly IMapper _mapper;

        public ClientRepository(ClientDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Client> GetClientByEmail(string email)
        {
            var clientEntity = await _context.Clients.AsNoTracking().FirstOrDefaultAsync(c => c.email == email);
            return _mapper.Map<Client>(clientEntity);
        }

        public async Task<Client> GetClientById(Guid id)
        {
            var clientEntity = await _context.Clients.FindAsync(id);
            if (clientEntity == null)
            {
                throw new Exception("No user under such ID");
            }
            return _mapper.Map<Client>(clientEntity);
        }

        public async Task<Client> GetClientByUserName(string userName)
        {
            var clientEntity = await _context.Clients.AsNoTracking().FirstOrDefaultAsync(c => c.userName == userName);
            if (clientEntity == null)
            {
                throw new Exception("Wrong username");
            }
            return _mapper.Map<Client>(clientEntity);
        }

        public async Task<bool> AddClient(Client client)
        {
            var clientEntity = _mapper.Map<ClientEntity>(client);
            await _context.Clients.AddAsync(clientEntity);
            await _context.SaveChangesAsync();
            return await _context.Clients.FindAsync(clientEntity.Id) != null;
        }

        public async Task<bool> AddFriend(Guid clientId, Guid friendsId)
        {
            var clientEntity = await _context.Clients.FindAsync(clientId);
            var friendsEntity = await _context.Clients.FindAsync(friendsId);

            if (clientEntity == null || friendsEntity == null)
            {
                return false;
            }

            clientEntity.friends?.Add(friendsId);
            _context.Clients.Update(clientEntity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RegisterClientAsync(Client client)
        {
            if (await _context.Clients.AnyAsync(c => c.email == client.email || c.userName == client.userName))
            {
                return false;
            }

            var clientEntity = _mapper.Map<ClientEntity>(client);
            await _context.Clients.AddAsync(clientEntity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
