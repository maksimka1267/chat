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

        // Метод для создания чата
        public override async Task<CreateChatResponse> CreateChat(CreateChatRequest request, ServerCallContext context)
        {
            var chatEntity = new ChatEntity
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Chaters = request.Chaters.Select(Guid.Parse).ToList(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                Messages = new List<Guid>()
            };

            var createdChat = await _repository.CreateChatAsync(chatEntity);

            var response = new CreateChatResponse
            {
                Chat = new ServiceChat.Chat
                {
                    Id = createdChat.Id.ToString(),
                    Name = createdChat.Name,
                    Chaters = { createdChat.Chaters.Select(c => c.ToString()) },
                    Messages = { createdChat.Messages.Select(m => m.ToString()) },
                    CreatedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(createdChat.CreatedAt),
                    UpdatedAt = createdChat.UpdatedAt.HasValue
                        ? Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(createdChat.UpdatedAt.Value)
                        : null
                }
            };

            return response;
        }

        // Метод для получения чата по его ID
        public override async Task<GetChatByIdResponse> GetChatById(GetChatByIdRequest request, ServerCallContext context)
        {
            var chatEntity = await _repository.GetChatByIdAsync(Guid.Parse(request.Id));
            if (chatEntity == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Chat not found"));
            }

            var response = new GetChatByIdResponse
            {
                Chat = new ServiceChat.Chat
                {
                    Id = chatEntity.Id.ToString(),
                    Name = chatEntity.Name,
                    Chaters = { chatEntity.Chaters.Select(c => c.ToString()) },
                    Messages = { chatEntity.Messages.Select(m => m.ToString()) },
                    CreatedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(chatEntity.CreatedAt),
                    UpdatedAt = chatEntity.UpdatedAt.HasValue
                        ? Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(chatEntity.UpdatedAt.Value)
                        : null
                }
            };

            return response;
        }

        // Метод для получения всех чатов
        public override async Task<GetlAllChatResponse> GetAllChats(GetlAllChatRequest request, ServerCallContext context)
        {
            var chatEntities = await _repository.GetAllChatsAsync();

            var response = new GetlAllChatResponse();
            response.Chats.AddRange(chatEntities.Select(chat => new ServiceChat.Chat
            {
                Id = chat.Id.ToString(),
                Name = chat.Name,
                Chaters = { chat.Chaters.Select(c => c.ToString()) },
                Messages = { chat.Messages.Select(m => m.ToString()) },
                CreatedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(chat.CreatedAt),
                UpdatedAt = chat.UpdatedAt.HasValue
                    ? Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(chat.UpdatedAt.Value)
                    : null
            }));

            return response;
        }

        // Метод для обновления чата
        public override async Task<UpdateChatResponse> UpdateChat(UpdateChatRequest request, ServerCallContext context)
        {
            // Получаем существующий чат из базы данных
            var chatEntity = await _repository.GetChatByIdAsync(Guid.Parse(request.Id));

            if (chatEntity == null)
            {
                return new UpdateChatResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Chat not found"
                };
            }

            // Обновляем только необходимые поля
            chatEntity.Name = request.Name;
            chatEntity.UpdatedAt = DateTime.UtcNow;

            // Сохраняем изменения
            try
            {
                await _repository.UpdateChatAsync(chatEntity);

                return new UpdateChatResponse
                {
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new UpdateChatResponse
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }


        // Метод для удаления чата
        public override async Task<DeleteChatResponse> DeleteChat(DeleteChatRequest request, ServerCallContext context)
        {
            await _repository.DeleteChatAsync(Guid.Parse(request.Id));

            return new DeleteChatResponse
            {
                IsSuccess = true
            };
        }

        // Метод для получения всех чатов клиента
        public override async Task<GetAllChatsOfClientResponse> GetAllChatsOfClient(GetAllChatsOfClientRequest request, ServerCallContext context)
        {
            var chatEntities = await _repository.GetAllChatsofClientAsync(Guid.Parse(request.ClientId));

            var response = new GetAllChatsOfClientResponse();
            response.Chats.AddRange(chatEntities.Select(chat => new ServiceChat.Chat
            {
                Id = chat.Id.ToString(),
                Name = chat.Name,
                Chaters = { chat.Chaters.Select(c => c.ToString()) },
                Messages = { chat.Messages.Select(m => m.ToString()) },
                CreatedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(chat.CreatedAt),
                UpdatedAt = chat.UpdatedAt.HasValue
                    ? Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(chat.UpdatedAt.Value)
                    : null
            }));

            return response;
        }
    }
}
