using System.IdentityModel.Tokens.Jwt;

namespace APIGateway.JWT
{
    public class JwtProvider:IJwtProvider
    {
        public string GetIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwtToken = handler.ReadJwtToken(token);

            var clientIdClaim =
                jwtToken.Claims.FirstOrDefault(c => c.Type == "clientId");

            if (clientIdClaim != null)
            {
                return clientIdClaim.Value;
            }

            throw new Exception("clientId not found in token");
        }
    }
}
