using APIGateway.Interface;
using APIGateway.Model;
using Microsoft.AspNetCore.Mvc;

namespace APIGateway.Controllers
{
    [Route("api/v1/chat")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatRepository _repository;

        public ChatController(IChatRepository repository)
        {
            _repository = repository;
        }

        // Метод для створення чату
        [HttpPost("create")]
        public async Task<IActionResult> CreateChat(string name, List<Guid> chaters)
        {

            var createdChat = await _repository.CreateChat(name, chaters);
            return CreatedAtAction(nameof(GetChatById), new { chatId = createdChat.Id }, createdChat);
        }

        // Метод для редагування чату
        [HttpPut("update")]
        public async Task<IActionResult> UpdateChat(Guid id,string name)
        {
            var updatedChat = await _repository.UpdateChat(id, name);
            return Ok(updatedChat);
        }

        // Метод для видалення чату
        [HttpDelete("delete/{chatId}")]
        public async Task<IActionResult> DeleteChat([FromRoute] Guid chatId)
        {
            var deletedChat = await _repository.DeleteChat(chatId);
            if (deletedChat == null)
            {
                return NotFound("Chat not found.");
            }

            return Ok("Chat successfully deleted.");
        }

        // Метод для отримання чату за ID
        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetChatById([FromRoute] Guid chatId)
        {
            var chat = await _repository.GetChatById(chatId);
            if (chat == null)
            {
                return NotFound("Chat not found.");
            }

            return Ok(chat);
        }

        // Метод для отримання всіх чатів користувача
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetChatsByUserId([FromRoute] Guid userId)
        {
            var chats = await _repository.GetChatsByUserId(userId);
            if (!chats.Any())
            {
                return NotFound("No chats found for this user.");
            }

            return Ok(chats);
        }
    }
}
