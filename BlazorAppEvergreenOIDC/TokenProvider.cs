﻿namespace BlazorAppEvergreenOIDC
{
    public class TokenProvider
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public string? IdToken { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}
