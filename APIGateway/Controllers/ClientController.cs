using APIGateway.Interface;
using APIGateway.JWT;
using APIGateway.Model;
using ClientService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace APIGateway.Controllers
{
    [Route("api/v1/client")]
    public class ClientController : ControllerBase
    {
        private readonly IClientRepository _repository;
        private readonly IJwtProvider _jwtProvider;
        public ClientController(IClientRepository repository, IJwtProvider jwtProvider)
        {
            _repository = repository;
            _jwtProvider = jwtProvider;
        }

        [HttpPost("Register")]

        public async Task<IActionResult> RegisterClient(string userName, string password, string email)
        {
            await _repository.RegisterClient(userName, password, email);

            return Ok();
        }
        [HttpPost("Login")]

        public async Task<IActionResult> Login(string email, string password)
        {
            var token = await _repository.Login(email, password);

            if (token == string.Empty)
            {
                return BadRequest(token);
            }

            Response.Cookies.Append("cookies", token);

            Guid id = Guid.Parse(_jwtProvider.GetIdFromToken(token));

            ClientDto clientDto = await _repository.GetPersonalCabinet(id);

            clientDto.Id = id;

            clientDto.Token = token;
            clientDto.Id = id;

            return Ok(clientDto);
        }
        [Authorize]
        [HttpPost("add-friend")]
        public async Task<IActionResult> AddFriend(Guid clientId, Guid friendId)
        {
            // Получаем пользователя по его ID
            Client user = await _repository.GetClientInfo(clientId);

            if (user == null)
            {
                return NotFound("Пользователь не найден");
            }

            // Проверяем, есть ли уже друг в списке друзей
            if (user.friends != null && user.friends.Contains(friendId))
            {
                return Conflict("Этот пользователь уже есть в списке ваших друзей");
            }

            // Добавляем друга
            bool result = await _repository.AddFriend(clientId, friendId);

            if (result)
            {
                return Ok("Друг успешно добавлен");
            }

            return StatusCode(500, "Не удалось добавить друга");
        }
        [Authorize]
        [HttpGet("list-friends/{clientId}")]
        public async Task<IActionResult> ListFriends(Guid clientId)
        {
            try
            {
                // Получаем личный кабинет клиента
                var clientDto = await _repository.GetPersonalCabinet(clientId);
                if (clientDto == null)
                {
                    return NotFound(new { Message = "Client not found" });
                }

                // Получаем список друзей клиента
                var friends = await _repository.GetListFriends(clientId);
                if (friends == null || !friends.Any())
                {
                    return Ok(new { Message = "No friends found" });
                }

                // Преобразуем список друзей в список их имен
                var friendNames = friends.Select(friend => friend.userName).ToList();

                return Ok(new { Friends = friendNames });
            }
            catch (Exception ex)
            {
                // Логирование ошибки и возврат 500 ошибки
                Console.WriteLine($"Error in ListFriends: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, "An internal error occurred");
            }
        }
        [Authorize]
        [HttpDelete("{clientId}")]
        public async Task<IActionResult> DeleteAccount(string clientId)
        {
            try
            {
                // Преобразуем строку в Guid
                if (!Guid.TryParse(clientId, out var clientGuid))
                {
                    return BadRequest(new { message = "Invalid clientId format." });
                }

                var isDeleted = await _repository.DeleteAccount(clientGuid);

                if (isDeleted)
                {
                    return Ok(new { message = "Account deleted successfully." });
                }
                else
                {
                    return StatusCode(500, new { message = "Failed to delete account." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }
        [Authorize]
        [HttpDelete("{clientId}/friends/{friendId}")]
        public async Task<IActionResult> DeleteFriend(string clientId, string friendId)
        {
            try
            {
                // Преобразуем строки clientId и friendId в Guid
                if (!Guid.TryParse(clientId, out var clientGuid))
                {
                    return BadRequest(new { message = "Invalid clientId format." });
                }

                if (!Guid.TryParse(friendId, out var friendGuid))
                {
                    return BadRequest(new { message = "Invalid friendId format." });
                }

                var isDeleted = await _repository.DeleteFriend(clientGuid, friendGuid);

                if (isDeleted)
                {
                    return Ok(new { message = "Friend deleted successfully." });
                }
                else
                {
                    return StatusCode(500, new { message = "Failed to delete friend." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }
    }
}
