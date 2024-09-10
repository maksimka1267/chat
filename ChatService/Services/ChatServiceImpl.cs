using System;
using System.Linq;
using System.Threading.Tasks;
using ChatService.Data;
using ChatService.Entity;
using ServiceChat;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using AutoMapper;
using ChatService.Repository;
using static ServiceChat.ChatService;

namespace ChatService.Services
{
    public class ChatServiceImpl : ChatServiceBase
    {
        private readonly IChatRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<ChatServiceImpl> _logger;

        public ChatServiceImpl(IChatRepository repository, IMapper mapper, ILogger<ChatServiceImpl> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public override async Task<ChatResponse> CreateChat(CreateChatRequest request, ServerCallContext context)
        {
            var chatEntity = _mapper.Map<ChatEntity>(request.Chat);
            var createdChat = await _repository.CreateChatAsync(chatEntity);
            var response = new ChatResponse
            {
                Chat = _mapper.Map<Chat>(createdChat)
            };
            return response;
        }

        public override async Task<ChatResponse> GetChatById(ChatIdRequest request, ServerCallContext context)
        {
            var chatEntity = await _repository.GetChatByIdAsync(Guid.Parse(request.Id));
            if (chatEntity == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Chat not found"));
            }
            return new ChatResponse
            {
                Chat = _mapper.Map<Chat>(chatEntity)
            };
        }

        public override async Task<ChatListResponse> GetAllChats(EmptyRequest request, ServerCallContext context)
        {
            var chatEntities = await _repository.GetAllChatsAsync();
            var response = new ChatListResponse();
            response.Chats.AddRange(chatEntities.Select(chat => _mapper.Map<Chat>(chat)));
            return response;
        }

        public override async Task<ChatResponse> UpdateChat(UpdateChatRequest request, ServerCallContext context)
        {
            var chatEntity = _mapper.Map<ChatEntity>(request.Chat);
            await _repository.UpdateChatAsync(chatEntity);
            return new ChatResponse
            {
                Chat = _mapper.Map<Chat>(chatEntity)
            };
        }

        public override async Task<EmptyRequest> DeleteChat(ChatIdRequest request, ServerCallContext context)
        {
            await _repository.DeleteChatAsync(Guid.Parse(request.Id));
            return new EmptyRequest();
        }
    }
}