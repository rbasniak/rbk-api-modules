namespace rbkApiModules.Commons.Core.UiDefinitions;

public class FormGroup
{
    public FormGroup()
    {
        Controls = new List<InputControl>();
    }

    public string Name { get; set; }

    public List<InputControl> Controls { get; set; }
}
