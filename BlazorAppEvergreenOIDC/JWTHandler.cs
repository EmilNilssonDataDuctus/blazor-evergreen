namespace BlazorAppEvergreenOIDC
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    public class JwtHandler
    {
        private readonly ILogger<JwtHandler> _logger;
        private readonly IConfiguration configuration;
        public JwtHandler(ILogger<JwtHandler> logger, IConfiguration configuration)
        {
            _logger = logger;
            this.configuration = configuration;
        }
        public string ExtractClaimFromToken(string jwtToken)
        {
            return ExtractClaimFromToken(jwtToken, "sub");
        }
        public string ExtractUserIdFromToken(string jwtToken)
        {
            return ExtractClaimFromToken(jwtToken, this.configuration.GetValue<string>("OpenIDConnect:UserIdClaim")!);
        }
        public string ExtractClaimFromToken(string jwtToken, string key)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(jwtToken) as JwtSecurityToken;

            if (jsonToken != null)
            {
                // Access the "key" claim (which typically represents the username)
                var usernameClaim = jsonToken.Claims.FirstOrDefault(claim => claim.Type == key);

                if (usernameClaim != null)
                {

                    // Loop through claims
                    foreach (var claim in jsonToken.Claims)
                    {
                        _logger.LogInformation($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
                    }

                    // Access the value of the "key" claim
                    return usernameClaim.Value;
                }
            }

            // Return null or throw an exception if the "sub" claim is not found
            return null;
        }
    }
}
