namespace rbkApiModules.Infrastructure.Models
{
    public class SimpleNamedEntity : BaseDataTransferObject
    {
        public SimpleNamedEntity()
        {

        }

        public SimpleNamedEntity(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Name { get; set; }

        public override string ToString()
        {
            return $"[{Id}] {Name}";
        }
    }

    public class SimpleNamedEntity<T>
    {
        public SimpleNamedEntity()
        {

        }

        public SimpleNamedEntity(T id, string name)
        {
            Id = id;
            Name = name;
        }

        public T Id { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return $"[{Id.ToString()}] {Name}";
        }
    }
}
