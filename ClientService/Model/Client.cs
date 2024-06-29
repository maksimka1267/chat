namespace ClientService.Model
{
    public class Client
    {
        private Client(string userName, string passwordHash, string email)
        {
            Id = Guid.NewGuid();
            this.userName = userName;
            password = passwordHash;
            this.email = email;
            SessionIds = new List<Guid>();
        }
        public Guid Id { get; private set; }

        public string? userName { get; set; }

        public string password { get; set; }

        public string? email { get; set; }

        public List<Guid> SessionIds { get; set; }

        public static Client Create(string userName, string passwordHash, string email)
        {
            return new Client(userName, passwordHash, email);
        }
    }
}
