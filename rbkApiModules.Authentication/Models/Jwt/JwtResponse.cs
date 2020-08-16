namespace rbkApiModules.Infrastructure
{
    /// <summary>
    /// Classe com o objeto para resposta do endpoint de login
    /// </summary>
    public class JwtResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
