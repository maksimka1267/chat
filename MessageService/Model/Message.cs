﻿namespace MessageService.Model
{
    public class Message
    {
        public Guid Id { get; private set; }

        public string? Text { get; set; }
        public Guid Author { get; set; }
        public DateTime Date { get; set; }
        public byte[]? Photo { get; set; }
        private Message(string? text, Guid author, byte[]? photo)
        {
            Text = Text;
            Author = Author;
            Date = DateTime.Now;
            Id = new Guid();
            Photo = photo;
        }
        public static Message Create(string text, Guid author, byte[] photo)
        {
            return new Message(text, author, photo);
        }
    }
}
