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
            ClientEntity? clientEntity = await _context.Clients
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.email == email);


            return _mapper.Map<Client>(clientEntity);
        }

        public async Task<Client> GetClientById(Guid id)
        {
            ClientEntity clientEntity = await _context.Clients.FindAsync(id) ?? throw new Exception("No user under such ID");


            return _mapper.Map<Client>(clientEntity);
        }


        public async Task<Client> GetClientByUserName(string userName)
        {
            ClientEntity clientEntity = await _context.Clients
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.userName == userName) ?? throw new Exception("Wrong username");

            return _mapper.Map<Client>(clientEntity);
        }

        public async Task<bool> AddClient(Client client)
        {
            ClientEntity clientEntity = _mapper.Map<ClientEntity>(client);

            await _context.Clients.AddAsync(clientEntity);
            await _context.SaveChangesAsync();

            var addedClientEntity = await _context.Clients.FindAsync(clientEntity.Id);

            return addedClientEntity != null;

        }

        public async Task<bool> AddFriend(Guid clientId, Guid friendsId)
        {
            ClientEntity? clientEntity = await _context.Clients.FindAsync(clientId);
            ClientEntity? friends =await _context.Clients.FindAsync(friendsId);
            if (clientEntity == null|| friends ==null)
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

            ClientEntity clientEntity = _mapper.Map<ClientEntity>(client);

            await _context.Clients.AddAsync(clientEntity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
