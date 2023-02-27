using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace rbkApiModules.Commons.Core.CodeGeneration;

public class EndpointInfo
{
    private MethodInfo _action;

    public EndpointInfo(ControllerInfo controller, MethodInfo action)
    {
        Name = action.Name;

        _action = action;

        UrlParameters = new List<PropertyInfo>();

        if (action.HasAttribute<HttpGetAttribute>())
        {
            Method = HttpMethod.Get;
            Route = action.GetAttribute<HttpGetAttribute>().Template;
        }

        if (action.HasAttribute<HttpPostAttribute>())
        {
            Method = HttpMethod.Post;
            Route = action.GetAttribute<HttpPostAttribute>().Template;
        }

        if (action.HasAttribute<HttpPutAttribute>())
        {
            Method = HttpMethod.Put;
            Route = action.GetAttribute<HttpPutAttribute>().Template;
        }

        if (action.HasAttribute<HttpDeleteAttribute>())
        {
            Method = HttpMethod.Delete;
            Route = action.GetAttribute<HttpDeleteAttribute>().Template;
        }

        if (action.HasAttribute<RouteAttribute>())
        {
            Route = action.GetAttribute<RouteAttribute>().Template;
        }

        var parameters = action.GetParameters();

        foreach (var parameter in parameters)
        {
            if (parameter.ParameterType.FullName.EndsWith("+Request"))
            {
                InputType = new TypeInfo(parameter.ParameterType);
            }
            else
            {
                if (Route != null && Route.Contains("{" + parameter.Name + "}"))
                {
                    UrlParameters.Add(new PropertyInfo(parameter.Name, parameter.ParameterType));
                }
                // TODO: Dar suporte a query parameters
            }
        }

        if (action.ReturnType.Name == nameof(Task) + "`1" && action.ReturnType.GenericTypeArguments.Length > 0)
        {
            var asyncType = action.ReturnType.GenericTypeArguments[0];

            if (asyncType.Name == nameof(ActionResult) + "`1" && asyncType.GenericTypeArguments.Length > 0)
            {
                var returnType = asyncType.GenericTypeArguments[0];

                ReturnType = new TypeInfo(returnType);
            }
        }

        IgnoreMode = action.GetCodeGenerationIgnoreMode();
    }

    public HttpMethod Method { get; set; }
    public string Name { get; set; }
    public string Route { get; set; }
    public List<PropertyInfo> UrlParameters { get; set; }
    public TypeInfo InputType { get; set; }
    public TypeInfo ReturnType { get; set; }

    public StoreBehavior StoreBehavior => _action.DeclaringType.GetNgxsStoreBehavior();

    public IgnoreMode IgnoreMode { get; set; }
    public bool IgnoreOnStatesGeneration => IgnoreMode == IgnoreMode.All || IgnoreMode == IgnoreMode.StateOnly;
    public bool IncludeInStatesGenertation => !IgnoreOnStatesGeneration;

    public override string ToString()
    {
        return $"{Method.ToString().ToUpper()} {Route}";
    }
}
