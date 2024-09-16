using System.ComponentModel.DataAnnotations;

namespace TestTaskApi.Domain.ViewModels
{
    public class MessageViewModel
    {
        [Required(ErrorMessage = "Это поле не должно быть пустым")]
        [Display(Name = "Ваше сообщение")]
        [MaxLength(128, ErrorMessage = "Сообщение не может превышать 128 символов")]
        public string Content { get; set; }

        public DateTime Timestamp { get; set; }
        public int SequenceNumber { get; set; }
    }
}
