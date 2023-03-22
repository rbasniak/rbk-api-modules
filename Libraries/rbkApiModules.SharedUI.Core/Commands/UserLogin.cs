using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using MediatR;
using rbkApiModules.Commons.Core;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;

namespace rbkApiModules.SharedUI.Core;

public class UserLogin
{
    public class Request : IRequest<CommandResponse> 
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class Handler : IRequestHandler<Request, CommandResponse>
    {
        private readonly RbkSharedUIAuthentication _sharedUIAuthentication;
        private readonly ILocalizationService _localization;

        public Handler(IOptions<RbkSharedUIAuthentication> sharedUIAuthentication, ILocalizationService localization)  
        {
            _sharedUIAuthentication = sharedUIAuthentication.Value;
            _localization = localization;
        }

        public async Task<CommandResponse> Handle(Request request, CancellationToken cancellationToken)
        {
            if (String.IsNullOrEmpty(_sharedUIAuthentication.Username))
            {
                throw new SafeException(_localization.GetValue(SharedUiMessages.Errors.CredentialsNotProperlyConfigured));
            }

            if (_sharedUIAuthentication.Username.ToLower() != request.Username.ToLower() || _sharedUIAuthentication.Password != request.Password)
            {
                throw new SafeException(_localization.GetValue(SharedUiMessages.Errors.InvalidCredentials));
            }

            var claims = new Dictionary<string, string[]>();

            var username = "";

            var response = new JwtResponse
            {
                AccessToken = GenerateEncodedToken(username, claims),
            };

            return await Task.FromResult(CommandResponse.Success(response));
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