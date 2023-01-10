using System.ComponentModel.DataAnnotations;

namespace rbkApiModules.Commons.Core.UiDefinitions;

public class DialogDataBuilderService
{
    public const int InputTextMaxLengh = 50;

    public List<FormGroup> Build(Type type, OperationType operation)
    {
        var results = new List<FormGroup>();

        var allInputs = new List<InputControl>();

        foreach (var property in type.GetProperties())
        {
            var required = type.GetAttributeFrom<RequiredAttribute>(property);
            var minlen = type.GetAttributeFrom<MinLengthAttribute>(property);
            var maxlen = type.GetAttributeFrom<MaxLengthAttribute>(property);

            var dialogData = type.GetAttributeFrom<DialogDataAttribute>(property);

            if (dialogData != null && (operation == dialogData.Operation || dialogData.Operation == OperationType.CreateAndUpdate))
            {
                var name = Char.ToLower(property.Name[0]) + property.Name.Substring(1);

                if (dialogData.Source == DataSource.ChildForm)
                {
                    allInputs.AddRange(Build(property.PropertyType, operation).SelectMany(x => x.Controls));
                }
                else
                {
                    var inputData = new InputControl(name, property.PropertyType, required, minlen, maxlen, dialogData);
                    allInputs.Add(inputData);
                }
            }
        }

        var groups = allInputs.GroupBy(x => x.Group);

        foreach (var groupedData in groups)
        {
            var group = new FormGroup();
            group.Name = groupedData.Key;
            group.Controls.AddRange(groupedData);

            results.Add(group);
        }

        return results;
    }
}

