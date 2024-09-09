using AutoMapper;
using ClientService.Repository;
using Grpc.Core;
using static ClientService.ClientAccount;

namespace ClientService.Services
{
    public class ClientServiceImpl:ClientAccountBase
    {
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;

        public ClientServiceImpl(IClientRepository clientRepository, IMapper mapper)
        {
            _clientRepository = clientRepository;
            _mapper = mapper;
        }

        public override async Task<RegisterClientResponse> RegisterClient(RegisterClientRequest request, ServerCallContext context)
        {
            var client = new Model.Client
            {
                email = request.Email,
                userName = request.UserName,
                password = request.Password, // Password should be hashed in a real scenario
            };

            var isSuccess = await _clientRepository.RegisterClientAsync(client);

            if (!isSuccess)
            {
                return new RegisterClientResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Client with the same email or username already exists"
                };
            }

            return new RegisterClientResponse { IsSuccess = true };
        }

        public override async Task<AuthenticateClientResponse> AuthenticateClient(AuthenticateClientRequest request, ServerCallContext context)
        {
            var client = await _clientRepository.GetClientByEmail(request.Email);

            if (client == null || client.password != request.Password) // In a real system, you'd verify hashed password
            {
                return new AuthenticateClientResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Invalid email or password"
                };
            }

            // Generate a token (simplified)
            var token = "generated_token"; // Replace with actual JWT token generation
            return new AuthenticateClientResponse
            {
                IsSuccess = true,
                Token = token
            };
        }

        public override async Task<GetClientInfoResponse> GetClientInfo(GetClientInfoRequest request, ServerCallContext context)
        {
            var clientId = Guid.Parse(request.Id);
            var client = await _clientRepository.GetClientById(clientId);

            if (client == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Client not found"));
            }

            var clientMessage = _mapper.Map<ClientMessage>(client);
            return new GetClientInfoResponse { Client = clientMessage };
        }

        public override async Task<AddFriendResponse> AddFriend(AddFriendRequest request, ServerCallContext context)
        {
            var client = await _clientRepository.GetClientByUserName(request.UserName);

            if (client == null)
            {
                return new AddFriendResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Client not found"
                };
            }

            var friend = await _clientRepository.GetClientByUserName(request.UserName);

            if (friend == null)
            {
                return new AddFriendResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Friend not found"
                };
            }

            var isSuccess = await _clientRepository.AddFriend(client.Id, friend.Id);
            return new AddFriendResponse { IsSuccess = isSuccess };
        }
    }
}
