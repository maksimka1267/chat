﻿namespace APIGateway.Model
{
    public class Client
    {
        public Guid Id { get; set; }

        public string userName { get; set; }

        public string password { get; set; }

        public string email { get; set; }
        public List<Guid>? friends { get; set; }
        public List<Guid>? chats { get; set; }
    }
}
