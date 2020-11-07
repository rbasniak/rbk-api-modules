using Newtonsoft.Json;
using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace rbkApiModules.UIAnnotations.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var builder = new DialogDataBuilderService();

            var results = builder.Build(typeof(DemoEntity), OperationType.Update);

            Debug.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));
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
        [DialogData(OperationType.Update, "Relacionamento 2", Group = "Dados Avançados", DefaultValue = null)]
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

        // dropdown from store
        [DialogData(OperationType.Update, "LinkedEntity", Group = "Dados Extras", Source = DataSource.Store, SourceName = "stateName", ForcedType = DialogControlTypes.DropDown)]
        public Guid LinkedEntityId { get; set; }
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
}
