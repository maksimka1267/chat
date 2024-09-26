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
        private readonly ILogger<ClientServiceImpl> _logger;

        public ClientServiceImpl(
           IClientRepository clientRepository,
           IPasswordHasher passwordHasher,
           IJwtProvider jwtProvider,
           ILogger<ClientServiceImpl> logger,
           IMapper mapper)

        {
            _clientRepository = clientRepository;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
            _mapper = mapper;
            _logger = logger;
        }

        public override async Task<RegisterClientResponse> RegisterClient(RegisterClientRequest request, ServerCallContext context)
        {
            // Проверка существования пользователя по email
            var existingClientByEmail = await _clientRepository.GetClientByEmail(request.Email);
            if (existingClientByEmail != null)
            {
                return new RegisterClientResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Пользователь с таким email уже существует"
                };
            }

            // Проверка существования пользователя по userName
            var existingClientByUserName = await _clientRepository.GetClientByUserName(request.UserName);
            if (existingClientByUserName != null)
            {
                return new RegisterClientResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Пользователь с таким username уже существует"
                };
            }

            var hashedPassword = _passwordHasher.Generate(request.Password);

            Client client = Client.Create(request.UserName, hashedPassword, request.Email);

            bool check = await _clientRepository.AddClient(client);

            if (check)
            {
                return new RegisterClientResponse { IsSuccess = true, ErrorMessage = string.Empty };
            }
            else
            {
                return new RegisterClientResponse { IsSuccess = false, ErrorMessage = "Не удалось зарегистрировать пользователя" };
            }
        }


        public override async Task<AuthenticateClientResponse> AuthenticateClient(AuthenticateClientRequest request, ServerCallContext context)
        {
            try
            {
                Client client = await _clientRepository.GetClientByEmail(request.Identifier);

                if (client == null)
                {
                    return new AuthenticateClientResponse { IsSuccess = false, Token = string.Empty, ErrorMessage = "No user found with this email" };
                }

                var result = _passwordHasher.Verify(request.Password, client.password);

                if (!result)
                {
                    return new AuthenticateClientResponse { IsSuccess = false, Token = string.Empty, ErrorMessage = "Wrong password" };
                }

                var token = _jwtProvider.GenerateToken(client);

                return new AuthenticateClientResponse { IsSuccess = true, Token = token };
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                Console.WriteLine($"Error in AuthenticateClient: {ex.Message}");
                throw new RpcException(new Status(StatusCode.Internal, "An internal error occurred"));
            }
        }


        public override async Task<GetClientInfoResponse> GetClientInfo(GetClientInfoRequest request, ServerCallContext context)
        {
            var clientId = Guid.Parse(request.Id);
            var client = await _clientRepository.GetClientById(clientId);

            if (client == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Клиент не найден"));
            }

            // Преобразуем Client в ClientMessage
            var clientMessage = _mapper.Map<ClientMessage>(client);

            return new GetClientInfoResponse { Client = clientMessage };
        }

        public override async Task<AddFriendResponse> AddFriend(AddFriendRequest request, ServerCallContext context)
        {
            // Преобразуем UserId из строки в Guid
            if (!Guid.TryParse(request.UserId, out var userId))
            {
                return new AddFriendResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Некорректный идентификатор пользователя"
                };
            }

            // Получаем информацию о клиенте
            var client = await _clientRepository.GetClientById(userId);

            if (client == null)
            {
                return new AddFriendResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Клиент не найден"
                };
            }

            // Преобразуем FriendId из строки в Guid
            if (!Guid.TryParse(request.FriendId, out var friendId))
            {
                return new AddFriendResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Некорректный идентификатор друга"
                };
            }

            // Получаем информацию о друге
            var friend = await _clientRepository.GetClientById(friendId);

            if (friend == null)
            {
                return new AddFriendResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Друг не найден"
                };
            }

            // Добавляем друга
            var isSuccess = await _clientRepository.AddFriend(client.Id, friend.Id);
            return new AddFriendResponse { IsSuccess = isSuccess };
        }


       public override async Task<AddChatToClientResponse> AddChatToClient(AddChatToClientRequest request, ServerCallContext context)
        {
            var client = await _clientRepository.GetClientById(Guid.Parse(request.ClientId));

            if (client == null)
            {
                return new AddChatToClientResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Клиент не найден"
                };
            }

            await _clientRepository.AddChatToClientAsync(client.Id, Guid.Parse(request.ChatId));
            return new AddChatToClientResponse
            {
                IsSuccess = true
            };
        }
        public override async Task<ListFriendResponse> ListFriend(ListFriendRequest request, ServerCallContext context)
        {
            // Преобразуем ClientId из строки в Guid
            if (!Guid.TryParse(request.ClientId, out var clientId))
            {
                return new ListFriendResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Некорректный идентификатор клиента"
                };
            }

            // Получаем список друзей
            var friends = await _clientRepository.GetListFriends(clientId);

            if (friends == null || !friends.Any())
            {
                return new ListFriendResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Нет друзей для данного клиента"
                };
            }

            // Преобразуем список друзей в список ClientMessage
            var friendMessages = _mapper.Map<List<ClientMessage>>(friends);

            return new ListFriendResponse
            {
                IsSuccess = true,
                Friends = { friendMessages }  // Преобразуем в repeated поле
            };
        }
        public override async Task<DeleteClientResponse> DeleteClient(DeleteClientRequest request, ServerCallContext context)
        {
            // Преобразуем строковый ClientId в Guid
            if (!Guid.TryParse(request.ClientId, out var clientId))
            {
                return new DeleteClientResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Invalid client ID format"
                };
            }

            // Ищем клиента по ID
            var client = await _clientRepository.GetClientById(clientId);
            if (client == null)
            {
                return new DeleteClientResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Client not found"
                };
            }

            // Удаляем клиента
            await _clientRepository.DeleteClient(client.Id);

            return new DeleteClientResponse
            {
                IsSuccess = true
            };
        }
        public override async Task<DeleteFriendResponse> DeleteFriend(DeleteFriendRequest request, ServerCallContext context)
        {
            // Проверка корректности ClientId
            if (!Guid.TryParse(request.ClientId, out var clientId))
            {
                return new DeleteFriendResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Invalid client ID format"
                };
            }

            // Проверка корректности FriendId
            if (!Guid.TryParse(request.FriendId, out var friendId))
            {
                return new DeleteFriendResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Invalid friend ID format"
                };
            }

            // Вызов репозитория для удаления друга
            var result = await _clientRepository.DeleteFriend(clientId, friendId);

            // Возвращаем результат операции
            return new DeleteFriendResponse
            {
                IsSuccess = result
            };
        }


    }
}
