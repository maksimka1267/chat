namespace APIGateway.Model
{
    public class ClientDto
    {
        public Guid Id { get; set; }

        public string Token { get; set; }
        public string userName { get; set; }

        public List<Guid>? ChatsId { get; set; }
    }
}
