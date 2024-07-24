namespace MessageService.Entity
{
    public class MessageEntity
    {
        public Guid Id { get; private set; }

        public string? Text { get; set; }
        public Guid? Author { get; set; }
        public DateTime? Date { get; set; }
        public byte[]? Photo { get; set; }
    }
}
