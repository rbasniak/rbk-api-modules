namespace rbkApiModules.Commons.Core.UiDefinitions;

public class FormDefinition
{
    public FormDefinition(List<FormGroup> createInputs, List<FormGroup> updateInputs)
    {
        Create = createInputs;
        Update = updateInputs;
    }

    public List<FormGroup> Create { get; }
    public List<FormGroup> Update { get; }
}
