using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using MediatR;
using rbkApiModules.Commons.Core;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace rbkApiModules.SharedUI.Core;

public class UserLogin
{
    public class Command : IRequest<CommandResponse> 
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class Handler : RequestHandler<Command, CommandResponse>
    {
        private readonly RbkSharedUIAuthentication _sharedUIAuthentication;

        public Handler(IOptions<RbkSharedUIAuthentication> sharedUIAuthentication)  
        {
            _sharedUIAuthentication = sharedUIAuthentication.Value;
        }

        protected override CommandResponse Handle(Command request)
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

            return CommandResponse.Success(response);
        }

        private string GenerateEncodedToken(string username, Dictionary<string, string[]> roles)
        {
            var claims = new List<Claim>
            {
                 new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub, username),
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
                issuer: "rbkApiModules.SharedUI.Core",
                audience: "rbkApiModules.SharedUI.Core",
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddYears(25),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }
    }
}