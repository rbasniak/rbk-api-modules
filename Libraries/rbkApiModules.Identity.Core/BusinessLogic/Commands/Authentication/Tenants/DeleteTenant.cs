using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;

namespace rbkApiModules.Identity.Core;

public class DeleteTenant
{
    public class Request : IRequest<CommandResponse>
    {
        private string _alias;

        public string Alias
        {
            get
            {
                return _alias;
            }
            set
            {
                _alias = value.ToUpper();
            }
        }
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly ITenantsService _tenantsService;

        public Validator(ITenantsService tenantsService, ILocalizationService localization)
        {
            _tenantsService = tenantsService;

            RuleFor(x => x.Alias)
                .IsRequired(localization)
                .MustAsync(ExistInDatabase)
                    .WithErrorCode(ValidationErrorCodes.NOT_FOUND)
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.TenantNotFound))
                    .WithName(localization.LocalizeString(AuthenticationMessages.Fields.TenantAlias));
        }

        private async Task<bool> ExistInDatabase(Request request, string alias, CancellationToken cancellation)
        {
            return await _tenantsService.FindAsync(alias, cancellation) != null;
        }
    }

    public class Handler : IRequestHandler<Request, CommandResponse>
    {
        private readonly ITenantsService _tenantsService;
        private readonly IRolesService _rolesService;
        private readonly IAuthService _authService;
        private readonly ILocalizationService _localization;

        public Handler(ITenantsService tenantsService, IAuthService authService, IRolesService rolesService, ILocalizationService localization)
        {
            _tenantsService = tenantsService;
            _rolesService = rolesService;
            _authService = authService;
            _localization = localization;
        }

        public async Task<CommandResponse> Handle(Request request, CancellationToken cancellation)
        {
            try
            {
                await _authService.DeleteUsersFromTenant(request.Alias, cancellation);

                await _rolesService.DeleteRolesFromTenant(request.Alias, cancellation);

                await _tenantsService.DeleteAsync(request.Alias, cancellation);
            }
            catch 
            {
                throw new SafeException(_localization.LocalizeString(AuthenticationMessages.Erros.CannotDeleteUsedTenant));
            }

            return CommandResponse.Success();
        }
    }
}