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

        public async Task<bool> DeleteFriend(Guid clientId, Guid friendsId)
        {
            // Находим клиента в базе данных по его идентификатору
            var clientEntity = await _context.Clients.FindAsync(clientId);
            // Если клиент не найден, возвращаем false
            if (clientEntity == null)
            {
                return false;
            }
            // Проверяем, есть ли указанный друг в списке друзей клиента
            if (clientEntity.friends != null && clientEntity.friends.Contains(friendsId))
            {
                // Удаляем друга из списка
                clientEntity.friends.Remove(friendsId);
                // Сохраняем изменения в базе данных
                await _context.SaveChangesAsync();
                return true;
            }
            // Если друг не найден в списке друзей, возвращаем false
            return false;
        }
        public async Task<bool> DeleteClient(Guid clientId)
        {
            // Находим клиента в базе данных по его идентификатору
            var clientEntity = await _context.Clients.FindAsync(clientId);
            // Если клиент не найден, возвращаем false
            if (clientEntity == null)
            {
                return false;
            }
            // Удаляем клиента из базы данных
            _context.Clients.Remove(clientEntity);
            // Сохраняем изменения в базе данных
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddChat(Guid clientId, Guid chatId)
        {
            // Получаем клиента по его ID
            var client = await _context.Clients.FindAsync(clientId);

            if (client == null)
            {
                // Клиент не найден, возвращаем false
                return false;
            }

            // Проверяем, существует ли такой чат уже у клиента
            if (client.chats != null && client.chats.Contains(chatId))
            {
                // Если чат уже добавлен, возвращаем true (чат уже связан с клиентом)
                return true;
            }

            // Если чатов у клиента еще нет, инициализируем список
            if (client.chats == null)
            {
                client.chats = new List<Guid>();
            }

            // Добавляем новый чат
            client.chats.Add(chatId);

            // Сохраняем изменения в базе данных
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
