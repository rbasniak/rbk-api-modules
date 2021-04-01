using rbkApiModules.Infrastructure.Models;
using rbkApiModules.UIAnnotations.Tests;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace rbkApiModules.UIAnnotations
{

    public class Tests2
    {
        [Fact]
        public void Test()
        {
            var service = new DialogDataBuilderService();
            var result = service.Build(typeof(MedicalInstitution), OperationType.Create);
        }
    }

    public class DemoEntity : BaseEntity
    {
        private HashSet<string> _list;

        private DemoEntity()
        {
            _list = new HashSet<string>();
        }

        // inputtext
        [Required, MinLength(5), MaxLength(15)]
        [DialogData(OperationType.CreateAndUpdate, "Nome", Group = "Dados Básicos")]
        public string Name { get; set; }

        // textarea
        [MinLength(5), MaxLength(500)]
        [DialogData(OperationType.CreateAndUpdate, "Descrição", Group = "Dados Básicos")]
        public string Description { get; set; }

        // mask
        [MinLength(11), MaxLength(11)]
        [DialogData(OperationType.CreateAndUpdate, "Descrição", Group = "Dados Básicos")]
        public string Phone { get; set; }

        // dropdown
        [Required]
        [DialogData(OperationType.Update, "Relacionamento 1", Group = "Dados Avançados")]
        public DemoEntity Navigation1 { get; set; }

        // dropdown
        [Required]
        [DialogData(OperationType.Update, "Relacionamento 2", Group = "Dados Avançados")]
        public DemoEntity Navigation2 { get; set; }

        // multi select
        [DialogData(OperationType.Update, "Items", Group = "Dados Avançados")]
        public List<DemoEntity> Children { get; set; }

        // numeric
        [DialogData(OperationType.Update, "Valor 1", Group = "Dados Extras")]
        public int Value { get; set; }

        // numeric
        [DialogData(OperationType.Update, "Valor 1", Group = "Dados Extras", DefaultValue = 19)]
        public double Value2 { get; set; }

        // radio
        [DialogData(OperationType.Update, "Ativo", Group = "Dados Extras", ForcedType = DialogControlTypes.Radio)]
        public bool IsActive { get; set; }

        // checkbox
        [DialogData(OperationType.Update, "Enviar e-mail", Group = "Dados Extras")]
        public bool SendEmail { get; set; }

        // dropdown
        [DialogData(OperationType.Update, "Fonte", Group = "Dados Extras")]
        public DataSource Source { get; set; }

        // form
        [DialogData(OperationType.Update, "IsActive", Group = "Dados Extras", Source = DataSource.ChildForm)]
        public NavigationEntity Delivery { get; set; }

        public IEnumerable<string> NonCrudProperty => _list?.ToList();


    }

    public class NavigationEntity : BaseEntity
    {
        [Required, MinLength(5), MaxLength(50)]
        [DialogData(OperationType.Update, "Endereço")]
        public string Endereco { get; set; }

        [Required]
        [DialogData(OperationType.Update, "Número")]
        public string Number { get; set; }

        [MinLength(5), MaxLength(50)]
        [DialogData(OperationType.Update, "Bairro")]
        public string Neighborhood { get; set; }
    }

    public class OtherEntity : BaseEntity
    {
        [Required, MinLength(5), MaxLength(50)]
        [DialogData(OperationType.Create, "Nome")]
        public string Name { get; set; }

        [Required]
        [DialogData(OperationType.Update, "Idade")]
        public int Age{ get; set; }

        [DialogData(OperationType.Update, "Tipo de Animal Preferido")]
        public Animals Input { get; set; }
    }

    public enum Animals
    {
        [Description("Nenhum")]
        None,

        [Description("Cachorros")]
        Dog,

        [Description("Gatos")]
        Cat,

        [Description("Peixes")]
        Fish,

        [Description("Pássaros")]
        Bird
    }
}

