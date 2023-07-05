using rbkApiModules.Commons.Core.UiDefinitions;
using rbkApiModules.Commons.Core;
using System.ComponentModel.DataAnnotations;

namespace Demo3.Models
{
    public class Project : BaseEntity
    {
        protected Project()
        {

        }

        public Project(string name, string code, string mdb)
        {
            Name = name;
            Code = code;
            Mdb = mdb;
        }

        [Required, MinLength(3), MaxLength(32)]
        [DialogData(OperationType.CreateAndUpdate, "Name")]
        public string Name { get; private set; }

        [Required, MinLength(3), MaxLength(16)]
        [DialogData(OperationType.CreateAndUpdate, "Code")]
        public string Code { get; private set; }

        [Required, MinLength(3), MaxLength(24)]
        [DialogData(OperationType.CreateAndUpdate, "Mdb")]
        public string Mdb { get; private set; }

        public void Update(string name, string code, string mdb)
        {
            Name = name;
            Code = code;
            Mdb = mdb;
        }
    }
}
