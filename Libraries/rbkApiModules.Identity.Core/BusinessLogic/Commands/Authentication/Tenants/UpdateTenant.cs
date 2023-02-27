using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;

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

            RuleFor(a => a.Alias)
                .IsRequired(localization)
                .MustAsync(ExistInDatabase).WithErrorCode(ValidationErrorCodes.NOT_FOUND).WithMessage(localization.GetValue("Tenant not found"))
                .WithName(localization.GetValue("Alias"))
                .DependentRules(() => 
                {
                    RuleFor(a => a.Name)
                       .IsRequired(localization)
                       .MustAsync(NameNotBeingUsed).WithMessage(localization.GetValue("Name already being used"))
                       .WithName(localization.GetValue("Name"));
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