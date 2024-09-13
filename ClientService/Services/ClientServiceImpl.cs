using AutoMapper;
using ClientService.Auth;
using ClientService.JWT;
using ClientService.Model;
using ClientService.Repository;
using Grpc.Core;
using static ClientService.ClientAccount;

namespace ClientService.Services
{
    public class ClientServiceImpl : ClientAccountBase
    {
        private readonly IClientRepository _clientRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;
        private readonly IMapper _mapper;

        public ClientServiceImpl(
           IClientRepository clientRepository,
           IPasswordHasher passwordHasher,
           IJwtProvider jwtProvider,
           IMapper mapper)

        {
            _clientRepository = clientRepository;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
            _mapper = mapper;
        }

        public override async Task<RegisterClientResponse> RegisterClient(RegisterClientRequest request, ServerCallContext context)
        {
            var client = new Model.Client
            {
                email = request.Email,
                userName = request.UserName,
                password = request.Password, // Пароль необходимо хешировать на реальном проекте
            };

            var isSuccess = await _clientRepository.RegisterClientAsync(client);

            if (!isSuccess)
            {
                return new RegisterClientResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Клиент с таким email или именем пользователя уже существует"
                };
            }

            return new RegisterClientResponse { IsSuccess = true };
        }

        public override async Task<AuthenticateClientResponse> AuthenticateClient(AuthenticateClientRequest request, ServerCallContext context)
        {
            Client client = await _clientRepository.GetClientByEmail(request.Identifier);

            if (client == null)
            {
                return new AuthenticateClientResponse { IsSuccess = false, Token = string.Empty, ErrorMessage = "No user found with this email" };
            }

            var result = _passwordHasher.Verify(request.Password, client.password);

            if (result == false)
            {
                return new AuthenticateClientResponse { IsSuccess = false, Token = string.Empty, ErrorMessage = "Wrong password" };
            }

            var token = _jwtProvider.GenerateToken(client);

            return new AuthenticateClientResponse { IsSuccess = true, Token = token };
        }

        public override async Task<GetClientInfoResponse> GetClientInfo(GetClientInfoRequest request, ServerCallContext context)
        {
            var clientId = Guid.Parse(request.Id);
            var client = await _clientRepository.GetClientById(clientId);

            if (client == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Клиент не найден"));
            }

            var clientMessage = _mapper.Map<ClientMessage>(client);
            return new GetClientInfoResponse { Client = clientMessage };
        }

        public override async Task<AddFriendResponse> AddFriend(AddFriendRequest request, ServerCallContext context)
        {
            var client = await _clientRepository.GetClientById(Guid.Parse(request.UserId));

            if (client == null)
            {
                return new AddFriendResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Клиент не найден"
                };
            }

            var friend = await _clientRepository.GetClientById(Guid.Parse(request.FriendsId));

            if (friend == null)
            {
                return new AddFriendResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Друг не найден"
                };
            }

            var isSuccess = await _clientRepository.AddFriend(client.Id, friend.Id);
            return new AddFriendResponse { IsSuccess = isSuccess };
        }

        public override async Task<AddChatResponse> AddChat(AddChatRequest request, ServerCallContext context)
        {
            var client = await _clientRepository.GetClientById(Guid.Parse(request.ClientId));

            if (client == null)
            {
                return new AddChatResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Клиент не найден"
                };
            }

            var isSuccess = await _clientRepository.AddChat(client.Id, Guid.Parse(request.ChatId));
            return new AddChatResponse { IsSuccess = isSuccess };
        }
    }
}
