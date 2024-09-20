namespace APIGateway.Model
{
    public class MessageModel
    {
        public Guid Id { get; set; }

        public string? Text { get; set; }
        public Guid Author { get; set; }
        public Guid ChatId { get; set; }
        public DateTime Date { get; set; }
        public byte[]? Photo { get; set; }
    }
}
