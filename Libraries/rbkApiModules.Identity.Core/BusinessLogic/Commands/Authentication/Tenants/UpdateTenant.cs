using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;

namespace rbkApiModules.Identity.Core;

public class UpdateTenant
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
                _alias = value?.ToUpper();
            }
        }
        public string Name { get; set; }
        public string Metadata { get; set; }
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
                    .WithMessage(localization.GetValue(AuthenticationMessages.Validations.TenantNotFound))
                    .WithName(localization.GetValue(AuthenticationMessages.Fields.TenantAlias))
                .DependentRules(() => 
                {
                    RuleFor(x => x.Name)
                       .IsRequired(localization)
                       .MustAsync(NameNotBeingUsed).WithMessage(localization.GetValue(AuthenticationMessages.Validations.NameAlreadyUsed))
                       .WithName(localization.GetValue(AuthenticationMessages.Fields.Name));
                });
        }

        private async Task<bool> ExistInDatabase(Request request, string alias, CancellationToken cancellation)
        {
            return await _tenantsService.FindAsync(alias, cancellation) != null;
        }

        private async Task<bool> NameNotBeingUsed(Request request, string name, CancellationToken cancellation)
        {
            var existingTenant = await _tenantsService.FindByNameAsync(name, cancellation);

            return  existingTenant == null || existingTenant.Alias == request.Alias;
        }
    }

    public class Handler : IRequestHandler<Request, CommandResponse>
    {
        private readonly ITenantsService _tenantsService;

        public Handler(ITenantsService tenantsService)
        {
            _tenantsService = tenantsService;
        }

        public async Task<CommandResponse> Handle(Request request, CancellationToken cancellation)
        {
            var tenant = await _tenantsService.UpdateAsync(request.Alias, request.Name, request.Metadata);

            return CommandResponse.Success(tenant);
        }
    }
}