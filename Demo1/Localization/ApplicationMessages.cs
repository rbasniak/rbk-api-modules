using System.ComponentModel;

namespace Demo1;

public class ApplicationMessages
{
    public class AuthenticationPolicies : ILocalizedResource
    {
        public enum Errors
        {
            [Description("You tried to login with the forbidden tenant")] TriedToLoginWithForbiddenTenant,
            [Description("You tried to login with the forbidden username")] TriedToLoginWithForbiddenUsername,
            [Description("The password must have at least 3 characteres")] PasswordMustHave3Characters,
        }
    }
}

public class MetadataValidationMessages : ILocalizedResource
{
    public enum General
    {
        [Description("Sector is required")] SectorIsRequired,
        [Description("Age must be greater than 18")] AgeMustBeGreaterThan18,
        [Description("Age is required")] AgeIsRequired,
        [Description("Age is not valid")] AgeIsNotValid,
    }
}