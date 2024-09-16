namespace TestTaskApi.Domain.Entity
{
    public class MessageModel
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public int SequenceNumber { get; set; }
    }
}
