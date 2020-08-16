using System.Collections.Generic;

namespace rbkApiModules.Infrastructure
{
    /// <summary>
    /// Classe para geração do access token JWT
    /// </summary>
    public class TokenGenerator
    {
        /// <summary>
        /// Método responsável por criar e codificar um novo access token
        /// </summary>
        /// <param name="jwtFactory">Token factory</param>
        /// <param name="username">Nome do usuário</param>
        /// <param name="roles">Permissões do usuário (claims)</param>
        /// <param name="refreshToken">Refresh token que será usado para renovar o token</param>
        /// <param name="jwtOptions">Dados necessários para geração do token</param>
        /// <returns></returns>
        public static JwtResponse Generate(IJwtFactory jwtFactory, string username, Dictionary<string, string[]> roles, string refreshToken)
        {
            var response = new JwtResponse
            {
                Token= jwtFactory.GenerateEncodedToken(username, roles),
                RefreshToken = refreshToken,
            };

            return response;
        }
    }
}
