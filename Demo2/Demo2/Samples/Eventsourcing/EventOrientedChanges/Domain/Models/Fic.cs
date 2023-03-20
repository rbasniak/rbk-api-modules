namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Models;

public class Fic
{
    public Fic()
    {

    }
    public Fic(Guid id, string name, Guid categoryId)
    {
        Id = id;
        Name = name;
        CategoryId = categoryId;
    }

    public virtual Guid Id { get; internal set; }
    public virtual string Name { get; internal set; }
    public virtual Guid CategoryId { get; internal set; }
}

