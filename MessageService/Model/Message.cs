public class Message
{
    public Guid Id { get; set; }

    public string? Text { get; set; }
    public Guid Author { get; set; }
    public Guid Chat { get; set; }
    public DateTime Date { get; set; }
    public byte[]? Photo { get; set; }

    public Message()
    {
        // Ініціалізація властивостей за замовчуванням, якщо потрібно
        Id = Guid.NewGuid();
        Date = DateTime.Now;
    }

    private Message(string? text, Guid author, Guid chat, byte[]? photo)
    {
        Text = text;
        Author = author;
        Chat = chat;
        Photo = photo;
        Date = DateTime.Now;
        Id = Guid.NewGuid();
    }

    public static Message Create(string text, Guid author, Guid chat, byte[] photo)
    {
        return new Message(text, author, chat, photo);
    }
}
