using AutoMapper;
using MessageService.Data;
using MessageService.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServiceChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ServiceChat.ChatService;

namespace MessageService.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly MessageDBContext _context;
        private readonly ChatServiceClient _chatclient;
        private readonly IMapper _mapper;
        private readonly ILogger<MessageRepository> _logger;

        public MessageRepository(MessageDBContext context, IMapper mapper, ILogger<MessageRepository> logger, ChatServiceClient chatclient)
        {
            _mapper = mapper;
            _context = context;
            _logger = logger;
            _chatclient = chatclient;
        }

        public async Task DeleteMessageAsync(Guid messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            //обновляем чат
            var request1 = new UpdateChatRequest
            {
                Id = message.Chat.ToString(),
            };
            var response1 = await _chatclient.UpdateChatAsync(request1);
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
        }

        public async Task<MessageEntity> GetMessageAsync(Guid messageId)
        {
            var messageEntity = await _context.Messages.FindAsync(messageId);
            if (messageEntity == null)
            {
                return null;
            }
            //обновляем чат
            var request1 = new UpdateChatRequest
            {
                Id = messageEntity.Chat.ToString(),
            };
            var response1 = await _chatclient.UpdateChatAsync(request1);
            return messageEntity;
        }

        public async Task<MessageEntity> SendMessageAsync(MessageEntity message)
        {
            try
            {
                _logger.LogInformation("Starting SendMessageAsync method");

                message.Date = DateTime.UtcNow;
                //обновляем чат
                var request1 = new AddMessageRequest
                {
                    Id = message.Chat.ToString(),
                    MessageId = message.Id.ToString(),
                };
                var response1 = await _chatclient.AddMessageAsync(request1);
                await _context.Messages.AddAsync(message);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Message saved to database successfully");

                var addedMessageEntity = await _context.Messages.FindAsync(message.Id);
                var result = addedMessageEntity != null;

                if (result)
                {
                    _logger.LogInformation("Message found in database after save");
                }
                else
                {
                    _logger.LogWarning("Message not found in database after save");
                }

                return addedMessageEntity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending a message");
                throw;
            }
        }

        public async Task UpdateMessageAsync(Guid id, string text)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                throw new Exception("Message not found"); 
            }
            message.Text = text;
            message.Date = DateTime.UtcNow;
            //обновляем чат
            var request1 = new UpdateChatRequest
            {
                Id = message.Chat.ToString(),
            };
            var response1 = await _chatclient.UpdateChatAsync(request1);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<MessageEntity>> FindMessagesAsync(string text)
        {
            var messageEntities = await _context.Messages
                .Where(m => m.Text.Contains(text))
                .ToListAsync();

            return messageEntities;
        }
    }
}
