namespace APIGateway.Model
{
    public class ChatModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Guid> Chaters { get; set; }
        public List<Guid>? Messages { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
