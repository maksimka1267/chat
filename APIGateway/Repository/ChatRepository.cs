using APIGateway.Interface;
using APIGateway.Model;
using AutoMapper;
using ServiceChat;
using static ServiceChat.ChatService;

namespace APIGateway.Repository
{
    public class ChatRepository : IChatRepository
    {
        private readonly IMapper _mapper;
        private readonly ChatServiceClient _client;
        public ChatRepository(IMapper mapper, ChatServiceClient client)
        {
            _mapper = mapper;
            _client = client;
        }

        // Метод створення чату
        public async Task<ChatModel> CreateChat(string name, List<Guid> chaters)
        {
            // Проверяем, существует ли уже чат с таким именем
            var allChatsRequest = new GetlAllChatRequest();
            var allChatsResponse = await _client.GetAllChatsAsync(allChatsRequest);

            if (allChatsResponse.Chats.Any(chat => chat.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new Exception($"Chat with name '{name}' already exists.");
            }

            // Если чата с таким именем нет, создаем новый чат
            CreateChatRequest request = new CreateChatRequest
            {
                Name = name
            };
            request.Chaters.AddRange(chaters.Select(c => c.ToString()));

            // Отправляем запрос на создание чата
            CreateChatResponse response = await _client.CreateChatAsync(request);

            // Проверяем результат создания
            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                throw new Exception(response.ErrorMessage);
            }

            // Возвращаем созданную модель чата
            return new ChatModel
            {
                Id = Guid.Parse(response.Chat.Id),
                Name = response.Chat.Name,
                Chaters = response.Chat.Chaters.Select(Guid.Parse).ToList(),
                Messages = response.Chat.Messages.Select(Guid.Parse).ToList(),
                CreatedAt = response.Chat.CreatedAt.ToDateTime(),
                UpdatedAt = response.Chat.UpdatedAt?.ToDateTime()
            };
        }



        // Метод оновлення чату
        public async Task<ChatModel> UpdateChat(Guid id, string name)
        {
            // Получаем чат по ID
            var getChatRequest = new GetChatByIdRequest { Id = id.ToString() };
            var chatResponse = await _client.GetChatByIdAsync(getChatRequest);

            // Проверяем, нашелся ли чат
            if (chatResponse.Chat == null)
            {
                throw new Exception("Chat not found");
            }

            // Обновляем только имя чата
            var updateChatRequest = new UpdateChatRequest
            {
                Id = chatResponse.Chat.Id, // сохраняем оригинальный Id
                Name = name,                // обновляем имя
            };

            // Отправляем запрос на обновление в сервис
            var updateResponse = await _client.UpdateChatAsync(updateChatRequest);

            // Проверяем, успешно ли обновился чат
            if (!updateResponse.IsSuccess)
            {
                throw new Exception(updateResponse.ErrorMessage);
            }

            // Возвращаем обновленную модель чата с сохраненными Chaters и Messages
            return new ChatModel
            {
                Id = Guid.Parse(chatResponse.Chat.Id),
                Name = chatResponse.Chat.Name, // обновленное имя
                Chaters = chatResponse.Chat.Chaters.Select(Guid.Parse).ToList(),
                Messages = chatResponse.Chat.Messages.Select(Guid.Parse).ToList(),
                CreatedAt = chatResponse.Chat.CreatedAt.ToDateTime(),
                UpdatedAt = DateTime.UtcNow    // обновленное время
            };
        }



        // Метод видалення чату
        public async Task<ChatModel> DeleteChat(Guid chatId)
        {
            var deleteChatRequest = new DeleteChatRequest
            {
                Id = chatId.ToString() // Перетворюємо GUID в рядок для запиту
            };

            var response = await _client.DeleteChatAsync(deleteChatRequest); // Викликаємо метод видалення
            if (!response.IsSuccess)
            {
                throw new Exception(response.ErrorMessage);
            }

            return null; // Успішне видалення, тому можна повернути null або кастомне повідомлення
        }

        // Метод отримання чату за ID
        public async Task<ChatModel> GetChatById(Guid chatId)
        {
            var getChatByIdRequest = new GetChatByIdRequest
            {
                Id = chatId.ToString()
            };

            var response = await _client.GetChatByIdAsync(getChatByIdRequest); // Викликаємо метод отримання чату
            if (response.Chat == null)
            {
                throw new Exception("Chat not found");
            }

            return _mapper.Map<ChatModel>(response.Chat); // Мапимо відповідь в модель
        }

        // Метод отримання всіх чатів користувача за його ID
        public async Task<IEnumerable<ChatModel>> GetChatsByUserId(Guid userId)
        {
            var getAllChatsRequest = new GetAllChatsOfClientRequest
            {
                ClientId = userId.ToString() // Перетворюємо GUID користувача в рядок
            };

            var response = await _client.GetAllChatsOfClientAsync(getAllChatsRequest); // Викликаємо метод отримання чатів користувача
            if (response.Chats == null || response.Chats.Count == 0)
            {
                return Enumerable.Empty<ChatModel>(); // Повертаємо порожній список, якщо немає чатів
            }

            return response.Chats.Select(chat => _mapper.Map<ChatModel>(chat)); // Мапимо список отриманих чатів у моделі
        }
    }
}
