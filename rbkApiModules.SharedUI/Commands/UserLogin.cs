using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Text;

namespace rbkApiModules.SharedUI
{
    /// <summary>
    /// Comando para login de usuário
    /// </summary>
    public class UserLogin
    {
        public class Command : IRequest<CommandResponse>, IPassword
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        /// <summary>
        /// Validador para o comando de login
        /// </summary>
        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                CascadeMode = CascadeMode.Stop; 
            }
        }

        public class Handler : BaseCommandHandler<Command>
        {
            private readonly RbkSharedUIAuthentication _sharedUIAuthentication;

            public Handler(IOptions<RbkSharedUIAuthentication> sharedUIAuthentication,
                IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
            {
                _sharedUIAuthentication = sharedUIAuthentication.Value;
            }

            protected override async Task<object> ExecuteAsync(Command request)
            {
                if (String.IsNullOrEmpty(_sharedUIAuthentication.Username))
                {
                    throw new SafeException("SharedUI credentials are not properly setup in the server");
                }

                if (_sharedUIAuthentication.Username.ToLower() != request.Username.ToLower() || _sharedUIAuthentication.Password != request.Password)
                {
                    throw new SafeException("Invalid credentials");
                }

                var claims = new Dictionary<string, string[]>();

                var username = "";

                var response = new JwtResponse
                {
                    AccessToken = GenerateEncodedToken(username, claims),
                };

                return await Task.FromResult(response);
            }

            private string GenerateEncodedToken(string username, Dictionary<string, string[]> roles)
            {
                var claims = new List<Claim>
                {
                     new Claim(JwtRegisteredClaimNames.Sub, username),
                     new Claim(ClaimTypes.Name, username),
                };

                foreach (var pair in roles)
                {
                    foreach (var value in pair.Value)
                    {
                        claims.Add(new Claim(pair.Key, value ?? ""));
                    }
                }

                var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("3RclogDLilc5HpioKsWl7hueJqNF4ISCED4wz4xSQukwQcy0dZ"));

                var jwt = new JwtSecurityToken(
                    issuer: "RbkApiModules.SharedUI",
                    audience: "RbkApiModules.SharedUI",
                    claims: claims,
                    notBefore: DateTime.UtcNow,
                    expires: DateTime.UtcNow.AddYears(25),
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                return encodedJwt;
            }
        }
    }
}