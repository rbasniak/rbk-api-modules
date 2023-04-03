using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;
using Serilog;
using System.Data;
using System.Text.Json.Serialization;
using static rbkApiModules.Commons.Core.Utilities.Localization.AuthenticationMessages;

namespace rbkApiModules.Identity.Core;

public class DeleteUser
{
    public class Request : AuthenticatedRequest, IRequest<CommandResponse>
    {
        public string Username { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly IAuthService _usersService;

        public Validator(IAuthService usersService, ILocalizationService localization)
        {
            _usersService = usersService;

            RuleFor(x => x.Username)
                .IsRequired(localization)
                .MustAsync(UserExistsOnDatabase)
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.UserNotFound))
                .WithName(localization.LocalizeString(AuthenticationMessages.Fields.User));
        }

        private async Task<bool> UserExistsOnDatabase(Request request, string username, CancellationToken cancellation)
        {
            var user = await _usersService.FindUserAsync(username, request.Identity.Tenant, cancellation);
            var result = user != null;
            return result;
        }
    }

    public class Handler : IRequestHandler<Request, CommandResponse>
    {
        private readonly IAuthService _usersService;
        private readonly IAvatarStorage _avatarStorage;
        private readonly RbkAuthenticationOptions _options;
        private readonly ILocalizationService _localization;
        private readonly IEnumerable<IUserMetadataService> _userMetadataService;

        public Handler(IAuthService usersService, IEnumerable<IUserMetadataService> userMetadataServices, IAvatarStorage avatarStorage, 
            RbkAuthenticationOptions options, ILocalizationService localization)
        {
            _options = options;
            _usersService = usersService;
            _localization = localization;
            _avatarStorage = avatarStorage;
            _userMetadataService = userMetadataServices;
        }

        public async Task<CommandResponse> Handle(Request request, CancellationToken cancellation)
        {
            try
            {
                await _usersService.DeleteUser(request.Username, request.Identity.Tenant, cancellation);
            }
            catch (Exception ex)
            {
                throw new SafeException(_localization.LocalizeString(AuthenticationMessages.Erros.CannotDeleteUser), ex, true);
            }

            return CommandResponse.Success();
        }
    }
}
