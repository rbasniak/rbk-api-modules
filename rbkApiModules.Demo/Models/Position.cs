using rbkApiModules.Infrastructure.Models;
using rbkApiModules.Payment.SqlServer;
using rbkApiModules.UIAnnotations;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
