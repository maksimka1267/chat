namespace ChatService.Model
{
    public class Chat
    {
        public Chat(string name, List<Guid> chaters)
        {
            Name = name;
            Chaters = chaters;
            Messages = new List<Guid>();
            Id= Guid.NewGuid();
            CreatedAt = DateTime.Now;
            UpdatedAt = null;
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Guid> Chaters { get; set; }
        public List<Guid>? Messages { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }


        public static Chat CreateChat(string name, List<Guid> chaters)
        {
            return new Chat(name, chaters);
        }
    }
}
