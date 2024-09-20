using APIGateway.Interface;
using APIGateway.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIGateway.Controllers
{
    [Authorize]
    [Route("api/v1/message")]
    public class MessageController : Controller
    {
        private readonly IMessageRepository _repository;
        public MessageController(IMessageRepository repository)
        {
            _repository = repository;
        }
        
        [HttpPost("Make-Message")]
        public async Task<IActionResult> MakeMessage(Guid author, Guid chatId, string? text, byte[]? photo)
        {
            MessageModel message = await _repository.SendMessage(author, chatId, text, photo);
            return Ok(message);
        }
        [HttpDelete("Delete-Message/{messageId}")]
        public async Task<IActionResult> DeleteMessage([FromRoute] Guid messageId)
        {
            await _repository.DeleteMessage(messageId);
            return Ok();
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateMessage(Guid messageId, string? text, byte[]? photo)
        {
            MessageModel message = await _repository.UpdateMessage(messageId, text);
            return Ok(message);
        }

        // Метод для отримання повідомлення за ID
        [HttpGet("{messageId}")]
        public async Task<IActionResult> GetMessageById([FromRoute] Guid messageId)
        {
            MessageModel message = await _repository.GetMessage(messageId);
            return Ok(message);
        }
    }
}
