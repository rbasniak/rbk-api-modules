
using rbkApiModules.Infrastructure.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace rbkApiModules.UIAnnotations.Tests
{
    public class MedicalInstitution : BaseEntity
    {
        public MedicalInstitution()
        {

        } 

        public virtual string Name { get; private set; }

        public virtual string Cnpj { get; private set; }

        public virtual string Logo { get; private set; }

        [DialogData(OperationType.CreateAndUpdate, "Endereço", Source = DataSource.ChildForm)]
        public virtual MedicalnstitutionAddress Address { get; private set; }

        public virtual string Email { get; private set; }

        public virtual string Phone { get; private set; }

        [MinLength(0), MaxLength(10)]
        public virtual string Extension { get; private set; }

    }

    public class MedicalnstitutionAddress
    {
        protected MedicalnstitutionAddress()
        {

        }

        public MedicalnstitutionAddress(string street, string number)
        {
            Street = street;
            Number = number;
        }

        public virtual string Street { get; private set; }

        [Required, MinLength(1), MaxLength(16)]
        [DialogData(OperationType.CreateAndUpdate, "Número")]
        public virtual string Number { get; private set; }

        [Required]
        [DialogData(OperationType.CreateAndUpdate, "Cidade", Source = DataSource.Store, SourceName = "cities")]
        public virtual Guid CityId { get; private set; }

        public void Update(string street, string number)
        {
            Street = street;
            Number = number;
        }
    }
}
