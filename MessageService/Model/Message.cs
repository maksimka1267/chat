namespace MessageService.Model
{
    public class Message
    {
        public Guid Id { get; private set; }

        public string? Text { get; set; }
        public Guid Author { get; set; }
        public Guid Chat {  get; set; }
        public DateTime Date { get; set; }
        public byte[]? Photo { get; set; }
        private Message(string? text, Guid author, Guid chat, byte[]? photo)
        {
            Text = Text;
            Author = Author;
            Date = DateTime.Now;
            Id = new Guid();
            Chat = chat;
            Photo = photo;
        }
        public static Message Create(string text, Guid author,Guid chat, byte[] photo)
        {
            return new Message(text, author,chat, photo);
        }
    }
}
