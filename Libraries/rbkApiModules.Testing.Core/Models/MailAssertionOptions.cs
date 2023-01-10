namespace rbkApiModules.Testing.Core;

public class MailAssertionOptions
{
    public List<string> ThingsToCheckOnTitle { get; private set; }
    public string Receiver { get; private set; }

    public MailAssertionOptions()
    {
        ThingsToCheckOnTitle = new List<string>();
    }

    public MailAssertionOptions ToAddress(string receiver)
    {
        Receiver = receiver;

        return this;
    }

    public MailAssertionOptions WithTileContaining(string text)
    {
        ThingsToCheckOnTitle.Add(text);

        return this;
    }
}
