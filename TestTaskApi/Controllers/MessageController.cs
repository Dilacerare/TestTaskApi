using Microsoft.AspNetCore.Mvc;
using System.Net;
using TestTaskApi.Domain.ViewModels;
using TestTaskApi.Service.Interfaces;

namespace TestTaskApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly ILogger<MessageController> _logger;

        public MessageController(IMessageService messageService, ILogger<MessageController> logger)
        {
            _messageService = messageService;
            _logger = logger;
        }

        /// <summary>
        /// Создание и отправка нового сообщения
        /// </summary>
        /// <param name="model">Сообщение</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(Domain.Entity.MessageModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateMessage([FromBody] MessageViewModel model)
        {
            if (ModelState.IsValid)
            {
                _logger.LogDebug("[MessageController.CreateMessage]: Модель данных валидна.");

                var response = await _messageService.Create(model);

                if (response.StatusCode == Domain.Enum.StatusCode.OK)
                {
                    _logger.LogInformation("[MessageController.CreateMessage]: Сообщение успешно создано.");
                    return Ok(response.Data);
                }
                else
                {
                    _logger.LogError($"[MessageController.CreateMessage]: Ошибка при создании сообщения: {response.Description}");
                    return StatusCode((int)response.StatusCode, response.Description);
                }
            }

            _logger.LogWarning("[MessageController.CreateMessage]: Модель данных не валидна.");
            return BadRequest(ModelState);
        }

        /// <summary>
        /// Получение всех сообщений за последние 10 минут
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<MessageViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllInLast10Min()
        {
            _logger.LogInformation("[MessageController.GetAllInLast10Min]: Запрос на получение сообщений за последние 10 минут.");

            var response = await _messageService.GetAllInLast10Min();
            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                _logger.LogInformation("[MessageController.GetAllInLast10Min]: Сообщения за последние 10 минут получены.");
                return Ok(response.Data);
            }
            else
            {
                _logger.LogError($"[MessageController.GetAllInLast10Min]: Ошибка при получении сообщений: {response.Description}");
                return StatusCode((int)response.StatusCode, response.Description);
            }
        }
    }

}
