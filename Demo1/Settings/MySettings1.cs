using rbkApiModules.Commons.Core.Features.ApplicationOptions;
using System.ComponentModel;

namespace Demo1;

public class MySettings1 : IApplicationOptions
{
    [DefaultValue("DefaultValue1")]
    public string Setting1 { get; set; }

    [DefaultValue(42)]
    public int Setting2 { get; set; }

    [DefaultValue(true)]
    public bool Setting3 { get; set; }

    public SubSettings SubSettings { get; set; } = new SubSettings();
}

public class SubSettings
{
    [DefaultValue("DefaultNestedValue1")]
    public string NestedSetting1 { get; set; } = "DefaultNestedValue1";

    [DefaultValue("DefaultNestedValue2")]
    public string NestedSetting2 { get; set; } = "DefaultNestedValue2";
}
