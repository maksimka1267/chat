using AutoMapper;
using Grpc.Core;
using ServiceMessage;
using MessageService.Repository;
using static ServiceMessage.MessageService;
using MessageService.Entity;

namespace MessageService.Services
{
    public class MessageServiceImpl : MessageServiceBase
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MessageServiceImpl> _logger;

        public MessageServiceImpl(IMessageRepository messageRepository, IMapper mapper, ILogger<MessageServiceImpl> logger)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public override async Task<SendMessageResponse> SendMessage(SendMessageRequest request, ServerCallContext context)
        {
            var message = new MessageEntity
            {
                Text = request.Text,
                Author = Guid.Parse(request.Author),
                Chat = Guid.Parse(request.ChatId),
                Date = DateTime.UtcNow,
                Photo = request.Photo != null ? request.Photo.ToByteArray() : null // Проверьте, что Photo не null
            };

            var createdMessage = await _messageRepository.SendMessageAsync(message);

            var responseMessage = _mapper.Map<ServiceMessage.Message>(message);
            return new SendMessageResponse { Message = responseMessage };
        }


        public override async Task<GetMessageResponse> GetMessage(GetMessageRequest request, ServerCallContext context)
        {
            var messageId = Guid.Parse(request.Id);
            var message = await _messageRepository.GetMessageAsync(messageId);

            if (message == null)
            {
                return new GetMessageResponse { ErrorMessage = "Message not found" };
            }

            var responseMessage = _mapper.Map<ServiceMessage.Message>(message);
            return new GetMessageResponse { Message = responseMessage };
        }

        public override async Task<FindMessageResponse> FindMessage(FindMessageRequest request, ServerCallContext context)
        {
            var messages = await _messageRepository.FindMessagesAsync(request.Text);
            var responseMessages = _mapper.Map<ServiceMessage.Message>(messages);

            return new FindMessageResponse
            {
                Messages = responseMessages,
                ErrorMessage = messages == null ? "Messages not found" : null
            };
        }

        public override async Task<DeleteMessageResponse> DeleteMessage(DeleteMessageRequest request, ServerCallContext context)
        {
            var messageId = Guid.Parse(request.Id);
            if (messageId == Guid.Empty)
            {
                return new DeleteMessageResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Message not found"
                };
            }
            try
            {
                await _messageRepository.DeleteMessageAsync(messageId);
                return new DeleteMessageResponse
                {
                    IsSuccess = true,
                };
            }catch (Exception ex)
            {
                return new DeleteMessageResponse
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
            
        }

        public override async Task<UpdateMessageResponse> UpdateMessage(UpdateMessageRequest request, ServerCallContext context)
        {
            var messageId = Guid.Parse(request.Id);
            if (messageId == Guid.Empty)
            {
                return new UpdateMessageResponse
                {
                    IsSuccess = false,
                    ErrorMessage="Message not found"
                };
            }
            try
            {
                await _messageRepository.UpdateMessageAsync(messageId, request.Text);
                return new UpdateMessageResponse
                {
                    IsSuccess = true,
                };
            }catch (Exception ex)
            {
                return new UpdateMessageResponse
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
