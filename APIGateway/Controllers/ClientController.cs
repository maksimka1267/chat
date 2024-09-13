using APIGateway.Interface;
using APIGateway.JWT;
using APIGateway.Model;
using Microsoft.AspNetCore.Mvc;

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
            /*var token = await _repository.Login(email, password);

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

            return Ok(clientDto);*/
            var toker = await _repository.Login(email, password);
            return Ok(toker);
        }
    }
}
