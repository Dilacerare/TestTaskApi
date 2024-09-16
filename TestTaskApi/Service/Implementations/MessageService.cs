using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using TestTaskApi.DAL.Interfaces;
using TestTaskApi.Domain.Entity;
using TestTaskApi.Domain.Enum;
using TestTaskApi.Domain.Handler;
using TestTaskApi.Domain.Response;
using TestTaskApi.Domain.ViewModels;
using TestTaskApi.Service.Interfaces;

namespace TestTaskApi.Service.Implementations
{
    public class MessageService : IMessageService
    {
        private readonly IBaseRepository<MessageModel> _messageRepository;
        private readonly IHubContext<MessageHub> _hubContext;
        private readonly ILogger<MessageService> _logger;

        public MessageService(IBaseRepository<MessageModel> messageRepository, 
            IHubContext<MessageHub> hubContext,
            ILogger<MessageService> logger)
        {
            _messageRepository = messageRepository;
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task<IBaseResponse<MessageModel>> Create(MessageViewModel model)
        {
            try
            {
                _logger.LogInformation("[MessageService.Create]: Начинается создание нового сообщения.");

                string content = model.Content;
                if (content.Length > 128)
                {
                    content = content.Substring(0, 128);
                    _logger.LogWarning("[MessageService.Create]: Содержимое сообщения было обрезано до 128 символов.");
                }

                int lastSequenceNumber = await _messageRepository.GetLatestSequenceNumber();
                _logger.LogDebug("[MessageService.Create]: Получен последний номер последовательности: {SequenceNumber}", lastSequenceNumber);

                var message = new MessageModel
                {
                    Content = content,
                    Timestamp = DateTime.UtcNow,
                    SequenceNumber = ++lastSequenceNumber
                };

                await _messageRepository.Create(message);
                _logger.LogInformation("[MessageService.Create]: Сообщение успешно сохранено в репозитории.");

                await _hubContext.Clients.All.SendAsync("ReceiveMessage", new MessageViewModel
                {
                    Content = content,
                    Timestamp = message.Timestamp,
                    SequenceNumber = message.SequenceNumber
                });
                _logger.LogInformation("[MessageService.Create]: Сообщение отправлено через SignalR.");

                return new BaseResponse<MessageModel>()
                {
                    Data = message,
                    Description = "Сообщение отправлено",
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[MessageService.Create]: Произошла ошибка при создании сообщения. {ex.Message}");
                return new BaseResponse<MessageModel>()
                {
                    StatusCode = StatusCode.InternalServerError,
                    Description = $"Внутренняя ошибка: {ex.Message}"
                };
            }
        }

        public async Task<BaseResponse<IEnumerable<MessageViewModel>>> GetAllInLast10Min()
        {
            _logger.LogInformation("[MessageService.GetAllInLast10Min]: Запрос на получение сообщений за последние 10 минут.");

            var baseResponse = new BaseResponse<IEnumerable<MessageViewModel>>();

            try
            {
                var now = DateTime.UtcNow;

                var messages = _messageRepository.GetAll().ToList();
                _logger.LogDebug("[MessageService.GetAllInLast10Min]: Получено {MessageCount} сообщений из репозитория.", messages.Count);

                var recentMessages = messages.Where(message => (now - message.Timestamp).TotalMinutes <= 10).ToList();
                _logger.LogDebug("[MessageService.GetAllInLast10Min]: Отфильтровано {MessageCount} сообщений за последние 10 минут.", recentMessages.Count);

                var messageViewModels = recentMessages.Select(message => new MessageViewModel
                {
                    Content = message.Content,
                    Timestamp = message.Timestamp,
                    SequenceNumber = message.SequenceNumber
                });

                return new BaseResponse<IEnumerable<MessageViewModel>>()
                {
                    Data = messageViewModels,
                    Description = "Сообщения за последние 10 минут получены",
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[MessageService.GetAllInLast10Min]: Произошла ошибка при получении сообщений. {ex.Message}");
                return new BaseResponse<IEnumerable<MessageViewModel>>()
                {
                    StatusCode = StatusCode.InternalServerError,
                    Description = $"Внутренняя ошибка: {ex.Message}"
                };
            }
        }
    }

}
