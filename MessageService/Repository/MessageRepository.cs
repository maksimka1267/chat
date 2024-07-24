using AutoMapper;
using MessageService.Data;
using MessageService.Entity;
using MessageService.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace MessageService.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly MessageDBContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(MessageDBContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<bool> DeleteMessageAsync(Guid messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message == null)
            {
                return false;
            }
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Message> GetMessageAsync(Guid messageId)
        {
            var messageEntity = await _context.Messages.FindAsync(messageId);
            if (messageEntity == null)
            {
                return null;
            }
            return _mapper.Map<Message>(messageEntity);
        }

        public async Task<bool> SendMessageAsync(Message message)
        {
            var messageEntity = _mapper.Map<MessageEntity>(message);
            await _context.Messages.AddAsync(messageEntity);
            await _context.SaveChangesAsync();
            var addedMessageEntity = await _context.Messages.FindAsync(messageEntity.Id);
            return addedMessageEntity != null;
        }

        public async Task<bool> UpdateMessageAsync(Guid id, string text)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return false;
            }
            message.Text = text;
            message.Date= DateTime.UtcNow;
            _context.Messages.Update(message);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Message>> FindMessagesAsync(string text)
        {
            var messageEntities = await _context.Messages
                .Where(m => m.Text.Contains(text))
                .ToListAsync();

            return _mapper.Map<IEnumerable<Message>>(messageEntities);
        }
    }
}