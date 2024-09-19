using ChatService.Entity;
using ChatService.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ChatService.Repository
{
    public class ChatRepository : IChatRepository
    {
        private readonly ChatDbContext _context;

        public ChatRepository(ChatDbContext context)
        {
            _context = context;
        }

        public async Task<ChatEntity> CreateChatAsync(ChatEntity chat)
        {
            await _context.Chat.AddAsync(chat);
            await _context.SaveChangesAsync();
            return chat;
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


        public async Task DeleteChatAsync(Guid id)
        {
            var chat = await GetChatByIdAsync(id);
            if (chat != null)
            {
                _context.Chat.Remove(chat);
                await _context.SaveChangesAsync();
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
    }
}
