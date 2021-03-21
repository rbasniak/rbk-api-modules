namespace rbkApiModules.Authentication
{
    /// <summary>
    /// Classe para armazenar nomes de propriedades padrão da especificação JWT
    /// </summary>
    public static class JwtClaimIdentifiers
    {
        /*
            If you want to use authorization using Roles direct on controllers,
            the variable value should be 'role'

            If you want to use authorization using Policies direct on controllers,
            the variable value should be 'rol'

            If you want to mix both, you need to add both claims to the token
        */
        public const string Roles = "rol";
        public const string Domain = "domain";
        public const string AuthenticationGroup = "auth-group";
        public const string Avatar = "avatar";
    }
}
