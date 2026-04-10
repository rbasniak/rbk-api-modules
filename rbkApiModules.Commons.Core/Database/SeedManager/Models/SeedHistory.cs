namespace rbkApiModules.Commons.Relational;

public class SeedHistory
{

    protected SeedHistory()
    {

    }

    public SeedHistory(string id, DateTime dateApplied)
    {
        Id = id;
        DateApplied = dateApplied;
    }

    public virtual string Id { get; private set; }

    public virtual DateTime DateApplied { get; private set; }
}