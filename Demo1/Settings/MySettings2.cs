using rbkApiModules.Commons.Core.Features.ApplicationOptions;
using System.ComponentModel;

namespace Demo1;

public class MySettings2 : IApplicationOptions
{
    [DefaultValue("DefaultValue1 from Settings 2")]
    public string Setting1 { get; set; } = "DefaultValue1 from Settings 2";

    [DefaultValue(19)]
    public int Setting2 { get; set; } = 19;

    [DefaultValue(true)]
    public bool Setting3 { get; set; } = true;
}
