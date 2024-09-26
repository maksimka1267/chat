using System;
using System.Threading.Tasks;
using APIGateway.Interface;
using APIGateway.Model;
using AutoMapper;
using Google.Protobuf;
using Grpc.Net.Client;
using ServiceChat;
using ServiceMessage;
using static ServiceChat.ChatService;
using static ServiceMessage.MessageService;

namespace APIGateway.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly MessageServiceClient _client;
        private readonly IMapper _mapper;

        public MessageRepository(MessageServiceClient client , IMapper mapper)
        {
            _client = client;
            _mapper = mapper;
        }

        public async Task<MessageModel> SendMessage(Guid author, Guid chatId, string? text, byte[]? photo)
        {
            // Создаем запрос с учетом возможных null значений
            var request = new SendMessageRequest
            {
                Author = author.ToString(),
                Text = text ?? string.Empty, // Если text null, заменяем его на пустую строку
                ChatId = chatId.ToString(),
                Photo = photo != null ? ByteString.CopyFrom(photo) : ByteString.Empty // Если photo null, используем пустой ByteString
            };

            // Отправляем запрос
            var response = await _client.SendMessageAsync(request);
            // Проверяем результат ответа
            if (response.Message == null)
            {
                throw new Exception(response.ErrorMessage);
            }

            // Возвращаем модель сообщения
            return new MessageModel
            {
                Id = Guid.Parse(response.Message.Id),
                Text = response.Message.Text,
                Author = Guid.Parse(response.Message.Author),
                ChatId = Guid.Parse(response.Message.ChatId),
                Photo = response.Message.Photo.ToByteArray(), // Конвертируем ByteString обратно в byte[]
                Date = DateTime.UtcNow,
            };
        }


        public async Task<MessageModel> GetMessage(Guid id)
        {
            var request = new GetMessageRequest { Id = id.ToString() };
            var response = await _client.GetMessageAsync(request);

            if (response == null)
            {
                throw new Exception(response.ErrorMessage);
            }
            MessageModel messageModel = _mapper.Map<MessageModel>(response);
            return new MessageModel
            {
                Id = messageModel.Id,
                Text = messageModel.Text,
                Author = messageModel.Author,
                ChatId = messageModel.ChatId,
                Photo = messageModel.Photo,
                Date =messageModel.Date,
            };
        }

        public async Task<MessageModel> UpdateMessage(Guid id, string text)
        {
            var request = new UpdateMessageRequest
            {
                Id = id.ToString(),
                Text = text
            };
            var response = await _client.UpdateMessageAsync(request);

            if (!response.IsSuccess)
            {
                throw new Exception(response.ErrorMessage);
            }
            var messageRequest = new GetMessageRequest { Id = id.ToString() };
            var messageResponse = await _client.GetMessageAsync(messageRequest);
            return new MessageModel
            {
                Id=Guid.Parse(messageResponse.Message.Id.ToString()),
                Author=Guid.Parse(messageResponse.Message.Author.ToString()),
                Text=messageResponse.Message.Text,
                ChatId=Guid.Parse(messageResponse.Message.ChatId.ToString()),
                Photo=messageResponse.Message.Photo.ToByteArray(),
                Date=DateTime.UtcNow,
            };
        }

        public async Task<bool> DeleteMessage(Guid id)
        {
            var request = new ServiceMessage.DeleteMessageRequest { Id = id.ToString() };
            var response = await _client.DeleteMessageAsync(request);

            if (!response.IsSuccess)
            {
                throw new Exception(response.ErrorMessage);
            }

            return response.IsSuccess;
        }
    }
}
