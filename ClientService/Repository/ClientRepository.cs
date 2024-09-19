using AutoMapper;
using ClientService.Data;
using ClientService.Entity;
using ClientService.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task<Client?> GetClientByEmail(string email)
        {
            var clientEntity = await _context.Client
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.email == email);

            return _mapper.Map<Client>(clientEntity);
        }

        public async Task<Client?> GetClientById(Guid id)
        {
            var clientEntity = await _context.Client.FindAsync(id);
            return _mapper.Map<Client>(clientEntity);
        }

        public async Task<Client?> GetClientByUserName(string userName)
        {
            var clientEntity = await _context.Client
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.userName == userName);

            return _mapper.Map<Client>(clientEntity);
        }

        public async Task<bool> AddClient(Client client)
        {
            var clientEntity = _mapper.Map<ClientEntity>(client);
            await _context.Client.AddAsync(clientEntity);
            var changes = await _context.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> RegisterClientAsync(Client client)
        {
            if (await _context.Client.AnyAsync(c => c.email == client.email || c.userName == client.userName))
            {
                return false;
            }

            var clientEntity = _mapper.Map<ClientEntity>(client);
            await _context.Client.AddAsync(clientEntity);
            var changes = await _context.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> AddFriend(Guid clientId, Guid friendId)
        {
            // Найти клиента и друга в базе данных
            var clientEntity = await _context.Client.FindAsync(clientId);
            var friendEntity = await _context.Client.FindAsync(friendId);

            // Проверить, существует ли клиент и друг
            if (clientEntity == null || friendEntity == null)
            {
                return false;
            }

            // Обновить список друзей клиента
            if (clientEntity.friends == null)
            {
                clientEntity.friends = new List<Guid>();
            }

            if (!clientEntity.friends.Contains(friendEntity.Id))
            {
                clientEntity.friends.Add(friendEntity.Id);
                _context.Client.Update(clientEntity);
            }

            // Обновить список друзей друга
            if (friendEntity.friends == null)
            {
                friendEntity.friends = new List<Guid>();
            }

            if (!friendEntity.friends.Contains(clientEntity.Id))
            {
                friendEntity.friends.Add(clientEntity.Id);
                _context.Client.Update(friendEntity);
            }

            // Сохранить изменения в базе данных
            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<bool> DeleteFriend(Guid clientId, Guid friendId)
        {
            // Получаем клиента из базы данных
            var client = await _context.Client.FirstOrDefaultAsync(c => c.Id == clientId);
            var friend = await _context.Client.FirstOrDefaultAsync(c => c.Id == friendId);

            if (client == null || friend == null)
            {
                return false;
            }

            // Проверяем, есть ли друг в списке клиента
            if (client.friends != null && client.friends.Contains(friendId))
            {
                // Удаляем друга из списка клиента
                client.friends.Remove(friendId);
            }

            // Проверяем, есть ли клиент в списке друзей друга
            if (friend.friends != null && friend.friends.Contains(clientId))
            {
                // Удаляем клиента из списка друзей друга
                friend.friends.Remove(clientId);
            }

            // Сохраняем изменения
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteClient(Guid clientId)
        {
            var clientEntity = await _context.Client.FindAsync(clientId);
            if (clientEntity == null)
            {
                return false;
            }

            _context.Client.Remove(clientEntity);
            var changes = await _context.SaveChangesAsync();
            return changes > 0;
        }

        /*public async Task<bool> AddChat(Guid clientId, Guid chatId)
        {
            var clientEntity = await _context.Clients.FindAsync(clientId);
            if (clientEntity == null)
            {
                return false;
            }

            if (clientEntity.chats == null)
            {
                clientEntity.chats = new List<Guid>();
            }

            if (!clientEntity.chats.Contains(chatId))
            {
                clientEntity.chats.Add(chatId);
                _context.Clients.Update(clientEntity);
                await _context.SaveChangesAsync();
            }

            return true;
        }
        */
        public async Task<List<Client>> GetListFriends(Guid clientId)
        {
            // Получаем все сущности клиентов, у которых в списке друзей есть данный clientId
            List<ClientEntity> clientEntities = await _context.Client
                .Where(c => c.friends.Contains(clientId))
                .ToListAsync();

            // Преобразуем клиентские сущности в объекты модели Client
            List<Client> clients = _mapper.Map<List<Client>>(clientEntities);

            return clients;
        }

    }
}
