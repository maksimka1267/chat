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
        public async Task<bool> AddChat(Guid chatnId, Guid clientId)
        {
            AddChatRequest request = new AddChatRequest
            {
                ChatId = chatnId.ToString(),
                ClientId = clientId.ToString()
            };
            AddChatResponse response = await _client.AddChatAsync(request);
            if (!response.IsSuccess)
            {
                throw new Exception(response.ErrorMessage);
            }
            return response.IsSuccess;
        }

        public async Task<bool> AddFriend(Guid friendId, Guid clientId)
        {
            AddFriendRequest request = new AddFriendRequest
            {
                FriendsId = friendId.ToString(),
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
                ChatsId = client.chats,
            };

        }
    }
}
