using ChatService.Entity;
using ChatService.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using static ClientService.ClientAccount;
using ClientService;

namespace ChatService.Repository
{
    public class ChatRepository : IChatRepository
    {
        private readonly ChatDbContext _context; 
        private readonly ClientAccountClient _accountClient;

        public ChatRepository(ChatDbContext context, ClientAccountClient accountClient)
        {
            _context = context;
            _accountClient = accountClient;
        }

        public async Task<ChatEntity> CreateChatAsync(ChatEntity chat)
        {
            try
            {
                await _context.Chat.AddAsync(chat);
                await _context.SaveChangesAsync();
                foreach (Guid clientId in chat.Chaters)
                {
                    var request = new AddChatToClientRequest
                    {
                        ClientId = clientId.ToString(),
                        ChatId = chat.Id.ToString()
                    };
                    await _accountClient.AddChatToClientAsync(request);
                }
            }
            catch (Exception ex)
            {
                // Логируйте ошибку
                Console.WriteLine($"Error creating chat: {ex.Message}");
                throw; // Перебросьте исключение для обработки выше
            }
            return chat;
        }


        public async Task DeleteChatAsync(Guid id)
        {
            var chat = await GetChatByIdAsync(id);
            if (chat != null)
            {
                // Удаляем ID чата из клиентов перед его удалением
                /*oreach (var clientId in chat.Chaters)
                {
                    var request = new RemoveChatToClientRequest
                    {
                        ClientId = clientId.ToString(), // Преобразуйте clientId в строку, если нужно
                        ChatId = chat.Id.ToString() // Преобразуйте chat.Id в строку, если нужно
                    };
                    await _accountClient.RemoveChatToClientAsync(request);
                }
                */
                _context.Chat.Remove(chat);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<ChatEntity> GetChatByIdAsync(Guid id)
        {
            return await _context.Chat.FindAsync(id);
        }
        public async Task<IEnumerable<ChatEntity>> GetAllChatsAsync()
        {
            return await _context.Chat.ToListAsync();
        }

        public async Task UpdateChatAsync(ChatEntity chatEntity)
        {
            var existingChat = await _context.Chat.FindAsync(chatEntity.Id);

            if (existingChat != null)
            {
                existingChat.Name = chatEntity.Name;
                existingChat.UpdatedAt = chatEntity.UpdatedAt;

                _context.Chat.Update(existingChat);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Chat not found");
            }
        }

        public async Task<IEnumerable<ChatEntity>> GetAllChatsofClientAsync(Guid clientId)
        {
            // Преобразуем clientId в строку для сравнения с Chaters
            var clientIdString = clientId.ToString();

            // Получаем все чаты, где клиент является участником
            var clientChats = await _context.Chat
                .Where(chat => chat.Chaters.Contains(Guid.Parse(clientIdString))) // Проверка, если клиент в списке Chaters
                .ToListAsync();

            return clientChats;
        }
        public async Task AddMessageAsync(Guid chatId, Guid messageId)
        {
            // Находим чат по его идентификатору
            var chat = await _context.Chat.Include(c => c.Messages).FirstOrDefaultAsync(c => c.Id == chatId);

            if (chat == null)
            {
                throw new Exception("Chat not found");
            }

            // Добавляем сообщение в чат
            chat.Messages.Add(messageId);

            // Сохраняем изменения в базе данных
            await _context.SaveChangesAsync();
        }
        public async Task DeleteMessageAsync(Guid chatId, Guid messageId)
        {
            var chat = await _context.Chat.Include(c => c.Messages).FirstOrDefaultAsync(c => c.Id == chatId);
            if (chat == null)
            {
                throw new Exception("Chat not found");
            }
            chat.Messages.Remove(messageId);
            await _context.SaveChangesAsync();
        }
    }
}
