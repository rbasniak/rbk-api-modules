using Serilog;
using System.ComponentModel;
using System.Text;
using System.Text.Json.Serialization;

namespace rbkApiModules.Commons.Core.CodeGeneration;

public class TypescriptModel
{
    public TypescriptModel(TypeInfo type)
    {
        Log.Information("Instantiating TypeScript model for type: {type}", type.Type.FullName);

        OriginalType = type.Type;
        // if (type.Name.Contains("Tree")) Debugger.Break();

        Name = type.Name;
        Filename = CodeGenerationUtilities.ToTypeScriptFileCase(type.Name);

        if (type.Name == "SimpleNamedEntity")
        {
            Filepath = null;
            ImportStatement = $"import {{ {type.Name} }} from 'ngx-smz-ui';";
        }
        else if (type.Name == "TreeNode")
        {
            Filepath = null;
            ImportStatement = $"import {{ {type.Name} }} from 'primeng/api';";
        }
        else
        {
            Filepath = Path.Join("models", $"{CodeGenerationUtilities.ToTypeScriptFileCase(type.Name)}.ts");
            ImportStatement = $"import {{ {type.Name} }} from '@models/{Filename}';";
        }


        Properties = new List<TypescriptProperty>();

        foreach (var property in type.Type.GetProperties().Where(x => x.GetAttribute<JsonIgnoreAttribute>() == null))
        {
            Properties.Add(new TypescriptProperty(property.Name, new TypeInfo(property.PropertyType), false));
        }
    }

    public string Name { get; set; }
    public Type OriginalType { get; set; }
    public List<TypescriptProperty> Properties { get; private set; }
    public string Filename { get; private set; }
    public string Filepath { get; private set; }
    public string ImportStatement { get; private set; }

    public string GenerateCode(List<TypescriptModel> models)
    {
        if (OriginalType.IsEnum)
        {
            return GenerateEnumCode(models);
        }
        else
        {
            return GenerateInterfaceCode(models);
        }
    }

    public string GenerateInterfaceCode(List<TypescriptModel> models)
    {
        var code = new StringBuilder();

        var externalReferences = new HashSet<string>();
        code.Append("{{EXTERNAL_REFERENCES}}");
        code.Append($"export interface {Name}" + " {" + Environment.NewLine);

        foreach (var property in Properties)
        {
            var optional = property.IsOptional ? "?" : "";
            code.AppendLine($"  {property.Declaration};");

            if (!property.Type.IsNative && property.Type.Name != Name)
            {
                externalReferences.Add(models.First(x => x.Name == property.Type.Name).ImportStatement);
            }
        }

        code.AppendLine("}");

        code = code.Replace("{{EXTERNAL_REFERENCES}}", String.Join(Environment.NewLine, externalReferences) + Environment.NewLine + Environment.NewLine);

        return code.ToString();
    }

    public string GenerateEnumCode(List<TypescriptModel> models)
    {
        var codePart1 = new StringBuilder();
        var codePart2 = new StringBuilder();
        var codePart3 = new StringBuilder();

        codePart1.Append("import { SimpleEntity } from 'ngx-smz-ui';" + Environment.NewLine + Environment.NewLine);

        var externalReferences = new HashSet<string>();
        codePart1.Append($"export enum {Name}" + " {" + Environment.NewLine);
        codePart2.Append($"export const {Name}Description: {{ [key in {Name}]: string }} = {{{Environment.NewLine}");
        codePart3.Append($"export const {Name}Values: SimpleEntity<number>[] = [{Environment.NewLine}");

        var names = Enum.GetNames(OriginalType);
        for (int i = 0; i < names.Length; i++)
        {
            var name = names[i];
            var field = OriginalType.GetField(name);
            var id = (int)Enum.Parse(OriginalType, name);

            var propertyName = System.Text.RegularExpressions.Regex.Replace(field.Name, "([a-z])([A-Z])", "$1_$2").ToUpper();

            var displayName = field.Name;
            var fds = field.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
            if (fds != null)
            {
                displayName = (fds as DescriptionAttribute).Description;
            }

            codePart1.AppendLine($"  {propertyName} = {id},");

            codePart2.AppendLine($"  [{Name}.{propertyName}]: '{displayName}',");

            codePart3.AppendLine($"  {{ id: {id}, name: '{displayName}' }},");

            /*
                Examples: 

                function cases() {

                  const response: { type: InputType } = { type: 2 };

                  const fromApiData = InputTypeDescription[response.type];
                  // fromApiData = 'Fluxo'

                  const fromTypescript = InputTypeDescription[InputType.FLOW];
                  // fromTypescript = 'Fluxo'

                }
            */
        }

        codePart1.AppendLine("}" + Environment.NewLine);
        codePart2.AppendLine("};" + Environment.NewLine);
        codePart3.AppendLine("];");

        return codePart1.ToString() + codePart2.ToString() + codePart3.ToString();
    }

    public override string ToString()
    {
        return Name;
    }
}
