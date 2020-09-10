namespace rbkApiModules.Authentication
{
    /// <summary>
    /// Classe com o objeto para resposta do endpoint de login
    /// </summary>
    public class JwtResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
