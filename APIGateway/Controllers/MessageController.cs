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
        
        /*[HttpPost("Make-Message")]
        public async Task<IActionResult> MakeMessage(Guid authorId, string? text, byte[]? photo)
        {
            MessageModel message = await _repository.CreateMessage(authorId, text, photo);
            return View(message);
        }*/
    }
}
