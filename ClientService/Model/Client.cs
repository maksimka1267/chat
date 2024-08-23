namespace ClientService.Model
{
    public class Client
    {
        public Guid Id { get; set; }

        public string userName { get; set; }

        public string password { get; set; }

        public string email { get; set; }
        public List<Guid>? friends { get; set; }
        public List<Guid>? chats { get; set; }
        private Client(string userName, string password, string email)
        {
            Id = new Guid();
            this.userName = userName;
            this.password = password;
            this.email = email;
            friends = new List<Guid>();
            chats = new List<Guid>();
        }

        public static Client Create(string userName, string passwordHash, string email)
        {
            return new Client(userName, passwordHash, email);
        }
    }
}
