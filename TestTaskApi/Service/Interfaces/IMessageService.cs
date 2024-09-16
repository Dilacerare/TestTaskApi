using TestTaskApi.Domain.Entity;
using TestTaskApi.Domain.Response;
using TestTaskApi.Domain.ViewModels;

namespace TestTaskApi.Service.Interfaces
{
    public interface IMessageService
    {
        Task<IBaseResponse<MessageModel>> Create(MessageViewModel model);

        Task<BaseResponse<IEnumerable<MessageViewModel>>> GetAllInLast10Min();

    }
}
