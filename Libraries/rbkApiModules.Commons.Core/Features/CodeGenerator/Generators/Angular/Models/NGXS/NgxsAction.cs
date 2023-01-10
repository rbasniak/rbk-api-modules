namespace rbkApiModules.Commons.Core.CodeGeneration;

public class NgxsAction
{
    public NgxsAction(string domainName, ActionType type, TypeInfo inputType, EndpointInfo endpoint)
    {
        Type = type;
        DomainName = domainName;
        InputType = inputType;
        Endpoint = endpoint;
    }

    public string DomainName { get; set; }
    public ActionType Type { get; set; }
    public TypeInfo InputType { get; set; }
    public EndpointInfo Endpoint { get; set; }

    public string GenerateCode()
    {
        var constructor = String.Empty;

        if (InputType != null)
        {
            if (InputType.Type != typeof(string))
            {
                constructor = $"{Environment.NewLine}{Environment.NewLine}    constructor(public data: {InputType.Name}) {{}}";
            }
            else
            {
                constructor = $"{Environment.NewLine}{Environment.NewLine}    constructor(public id: string) {{}}";
            }
        }

        var code = $@"  export class {Type} {{
    public static readonly type = '[{DomainName} API] {Type}';{constructor}
  }}";
        return code;
    }
}
