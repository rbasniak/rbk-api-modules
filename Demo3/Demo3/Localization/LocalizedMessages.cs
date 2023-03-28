using rbkApiModules.Commons.Core.Utilities.Localization;
using System.ComponentModel;

namespace Demo3.Localization;

public class ApplicationMessages2
{
    public class AuthenticationPolicies : ILocalizedResource
    {
        public enum Errors
        {
            [Description("You tried to login with the forbidden tenant")] TriedToLoginWithForbiddenTenant,
            [Description("You tried to login with the forbidden username")] TriedToLoginWithForbiddenUsername,
            [Description("The password must have at least 3 characteres")] PasswordMustHave3Characters,
        }

        public enum Validations
        {
            [Description("You tried to login with the forbidden tenant")] TriedToLoginWithForbiddenTenant,
            [Description("You tried to login with the forbidden username")] TriedToLoginWithForbiddenUsername,
            [Description("The password must have at least 3 characteres")] PasswordMustHave3Characters,
        }
    }

    public class OtherPolicies : ILocalizedResource
    {
        public enum Errors
        {
            [Description("You tried to login with the forbidden tenant")] TriedToLoginWithForbiddenTenant,
            [Description("You tried to login with the forbidden username")] TriedToLoginWithForbiddenUsername,
            [Description("The password must have at least 3 characteres")] PasswordMustHave3Characters,
        }

        public enum Validations
        {
            [Description("You tried to login with the forbidden tenant")] TriedToLoginWithForbiddenTenant,
            [Description("You tried to login with the forbidden username")] TriedToLoginWithForbiddenUsername,
            [Description("The password must have at least 3 characteres")] PasswordMustHave3Characters,
        }
    }
}