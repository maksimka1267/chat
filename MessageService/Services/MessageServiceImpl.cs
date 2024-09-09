using AutoMapper;
using Grpc.Core;
using MessageService.Protos;
using MessageService.Repository;
using static MessageService.Protos.MessageService;

namespace MessageService.Services
{
    public class MessageServiceImpl:MessageServiceBase
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public MessageServiceImpl(IMessageRepository messageRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        public override async Task<SendMessageResponse> SendMessage(SendMessageRequest request, ServerCallContext context)
        {
            var message = _mapper.Map<Model.Message>(request.Message);
            var isSent = await _messageRepository.SendMessageAsync(message);

            return new SendMessageResponse { IsSent = isSent };
        }

        public override async Task<GetMessageResponse> GetMessage(GetMessageRequest request, ServerCallContext context)
        {
            var messageId = Guid.Parse(request.Id);
            var message = await _messageRepository.GetMessageAsync(messageId);

            if (message == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Message not found"));
            }

            var responseMessage = _mapper.Map<Protos.Message>(message);
            return new GetMessageResponse { Message = responseMessage };
        }

        public override async Task<FindMessageResponse> FindMessage(FindMessageRequest request, ServerCallContext context)
        {
            var messages = await _messageRepository.FindMessagesAsync(request.Text);
            var responseMessages = _mapper.Map<IEnumerable<Protos.Message>>(messages);

            var response = new FindMessageResponse();
            response.Messages.AddRange(responseMessages);
            return response;
        }

        public override async Task<DeleteMessageResponse> DeleteMessage(DeleteMessageRequest request, ServerCallContext context)
        {
            var messageId = Guid.Parse(request.Id);
            var isSuccess = await _messageRepository.DeleteMessageAsync(messageId);

            return new DeleteMessageResponse { IsSuccess = isSuccess };
        }

        public override async Task<UpdateMessageResponse> UpdateMessage(UpdateMessageRequest request, ServerCallContext context)
        {
            var messageId = Guid.Parse(request.Id);
            var isSuccess = await _messageRepository.UpdateMessageAsync(messageId, request.Text);

            return new UpdateMessageResponse { IsSuccess = isSuccess };
        }
    }
}
