using APIGateway.Interface;
using APIGateway.Model;
using AutoMapper;
using ClientService;
using static ClientService.ClientAccount;


namespace APIGateway.Repository
{
    public class ClientRepository : IClientRepository
    {
        private readonly IMapper _mapper;
        private readonly ClientAccountClient _client;
        
        public ClientRepository(IMapper mapper, ClientAccountClient client)
        {
            _mapper = mapper;
            _client = client;
        }

        public async Task<bool> AddFriend(Guid friendId, Guid clientId)
        {
            AddFriendRequest request = new AddFriendRequest
            {
                FriendId = friendId.ToString(),
                UserId = clientId.ToString()
            };
            AddFriendResponse response = await _client.AddFriendAsync(request);
            if (!response.IsSuccess)
            {
                throw new Exception(response.ErrorMessage);
            }
            return response.IsSuccess;
        }

        public async Task<bool> RegisterClient(string username, string password, string email)
        {
            RegisterClientRequest request = new RegisterClientRequest
            {
                UserName = username,
                Password = password,
                Email = email,
            };

            RegisterClientResponse response = await _client.RegisterClientAsync(request);

            if (!response.IsSuccess)
            {
                throw new Exception(response.ErrorMessage);
            }

            return response.IsSuccess;
        }

        public async Task<string> Login(string email, string password)
        {
            AuthenticateClientRequest request = new AuthenticateClientRequest
            {
                Identifier = email,
                Password = password
            };

            AuthenticateClientResponse response = await _client.AuthenticateClientAsync(request);

            if (!response.IsSuccess)
            {
                throw new Exception(response.ErrorMessage);
            }

            return response.Token;
        }

        public async Task<ClientDto> GetPersonalCabinet(Guid id)
        {
            GetClientInfoRequest request = new GetClientInfoRequest
            {
                Id = id.ToString(),
            };

            GetClientInfoResponse response = await _client.GetClientInfoAsync(request);

            Client client = _mapper.Map<Client>(response);

            return new ClientDto
            {
                Id = client.Id,
                userName = client.userName,
            };

        }

        public async Task<Client> GetClientInfo(Guid clientId)
        {
            var request = new GetClientInfoRequest
            {
                Id = clientId.ToString()
            };

            // Вызов метода сервиса для получения информации о клиенте
            var response = await _client.GetClientInfoAsync(request);

            if (response == null || response.Client == null)
            {
                throw new Exception("Client not found");
            }

            var client = _mapper.Map<Client>(response.Client);

            return client;
        }
        public async Task<List<Client>> GetListFriends(Guid clientId)
        {
            // Создайте запрос для получения списка друзей
            ListFriendRequest request = new ListFriendRequest
            {
                ClientId = clientId.ToString()
            };

            // Вызовите метод сервиса для получения списка друзей
            ListFriendResponse response = await _client.ListFriendAsync(request);

            if (!response.IsSuccess)
            {
                throw new Exception(response.ErrorMessage);
            }

            // Преобразуйте список друзей из ClientMessage в ClientDto
            var friends = _mapper.Map<List<Client>>(response.Friends);

            return friends;
        }

        public async Task<bool> DeleteAccount(Guid clientId)
        {
            // Создаем запрос для gRPC-сервиса
            var request = new DeleteClientRequest
            {
                ClientId = clientId.ToString()
            };

            try
            {
                // Отправляем запрос на удаление через gRPC
                var response = await _client.DeleteClientAsync(request);

                if (!response.IsSuccess)
                {
                    throw new Exception($"Failed to delete account: {response.ErrorMessage}");
                }

                return response.IsSuccess;
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                Console.WriteLine($"Error deleting account for clientId {clientId}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteFriend(Guid clientId, Guid friendsId)
        {
            var request = new DeleteFriendRequest
            {
                ClientId = clientId.ToString(),
                FriendId = friendsId.ToString()
            };
            try
            {
                var response = await _client.DeleteFriendAsync(request);
                if (!response.IsSuccess)
                {
                    throw new Exception(response.ErrorMessage);
                }
                return response.IsSuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting friend for clientId {clientId}: {ex.Message}");
                return false;
            }
        }
    }
}
