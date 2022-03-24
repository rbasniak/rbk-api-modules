using System.ComponentModel;

namespace rbkApiModules.Demo.Models
{
    public enum Position
    {
        Supervisor = 10,
        Manager = 20,
        Owner = 30,
        CEO = 40,
        SuperManager = 50,
        [Description("Acionista majoritário")]
        SuperDuperManager
    }
}
