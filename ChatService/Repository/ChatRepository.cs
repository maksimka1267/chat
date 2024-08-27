using ChatService.Entity;
using ChatService.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            await _context.Chats.AddAsync(chat);
            await _context.SaveChangesAsync();
            return chat;
        }

        public async Task<ChatEntity> GetChatByIdAsync(Guid id)
        {
            return await _context.Chats.FindAsync(id);
        }

        public async Task<IEnumerable<ChatEntity>> GetAllChatsAsync()
        {
            return await _context.Chats.ToListAsync();
        }

        public async Task UpdateChatAsync(ChatEntity chat)
        {
            _context.Chats.Update(chat);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteChatAsync(Guid id)
        {
            var chat = await GetChatByIdAsync(id);
            if (chat != null)
            {
                _context.Chats.Remove(chat);
                await _context.SaveChangesAsync();
            }
        }
    }
}
