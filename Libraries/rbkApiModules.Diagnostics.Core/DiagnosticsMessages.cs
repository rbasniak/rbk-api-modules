using rbkApiModules.Commons.Core.Utilities.Localization;
using System.ComponentModel;

namespace rbkApiModules.Diagnostics.Core;

public class DiagnosticsMessages : ILocalizedResource
{
    public enum Validation
    {
        [Description("End date must be greater or equal to start date")] EndDateMustBeGreaterOrEqualToStartDate,
    }
}