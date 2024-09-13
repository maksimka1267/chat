namespace APIGateway.JWT
{
    public interface IJwtProvider
    {
        string GetIdFromToken(string token);
    }
}
