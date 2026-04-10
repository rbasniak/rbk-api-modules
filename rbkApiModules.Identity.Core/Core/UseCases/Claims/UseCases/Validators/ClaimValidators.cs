using rbkApiModules.Identity.Core;

internal static class ClaimsValidators
{
    public static IRuleBuilderOptions<T, Guid> ClaimExistOnDatabase<T>(this IRuleBuilder<T, Guid> rule, IClaimsService claims, ILocalizationService localization)
    {
        return rule
            .MustAsync(async (claimId, cancellationToken) =>
            {
                var result =  await claims.FindAsync(claimId, cancellationToken) != null;

                return result;
            })
            .WithMessage(command => localization.LocalizeString(AuthenticationMessages.Validations.ClaimNotFound));
    }
}

