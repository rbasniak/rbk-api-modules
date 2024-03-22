using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Commons.Core.Localization;
using Serilog;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace rbkApiModules.Commons.Core.CodeGeneration;

public class AngularCodeGenerator
{
    private string _basePath = "";
    private string _projectId = "";
    private readonly ILocalizationService _localization;

    public AngularCodeGenerator(string projectId, string basePath, ILocalizationService localization)
    {
        Log.Information("Code generator initialized for {project} in {path}", projectId, basePath);

        _localization = localization;

        _basePath = basePath;
        _projectId = projectId;
    }

    public void Generate(Assembly[] assemblies = null)
    {
        Log.Information("Begining code generation");

        var servicesFolder = Path.Combine(_basePath, "services", "api");
        Directory.CreateDirectory(servicesFolder);

        var controllers = GetControllers(_projectId, assemblies);

        Log.Information("Found {amount} controllers", controllers.Length);

        foreach ( var controller in controllers)
        {
            Log.Information("CONTROLLER: {controller} ROUTE: {route}", controller.Name, controller.Route);

            foreach (var action in controller.Endpoints)
            {
                Log.Information("  ENDPOINT: {endpoint} ROUTE: {route} GENERATE STATE: {generateState} IGNORE MODE: {ignoreMode} STATE BEHAVIOR: {stateBehavior}",
                    action.Name, action.Route, action.IncludeInStatesGenertation, action.IgnoreMode, action.StoreBehavior);

                if (action.InputType != null)
                {
                    Log.Information("    INPUT: {inputName} ({inputType})", action.InputType.Name, action.InputType.Type.FullName);
                }
                else
                {
                    Log.Information("    INPUT: none");
                }

                if (action.ReturnType != null)
                {
                    Log.Information("    OUTPUT: {outputName} ({outputType})", action.ReturnType.Name, action.ReturnType.Type.FullName);
                }
                else
                {
                    Log.Information("    OUTPUT: none");
                }

                if (action.UrlParameters != null)
                {
                    Log.Information("    PARAMETERS: ");
                }

                foreach (var parameter in action.UrlParameters)
                {
                    Log.Information("      NAME: {paramName} TYPE: {paramType} ({paramFullType})", parameter.Name, parameter.Type.Name, parameter.Type.Type.FullName);
                }
            }
        }

        var models = GetForcedTypescriptModels(_projectId);

        var index = 0;

        // Select the 1st level models (inputs for endpoints)

        foreach (var controller in controllers)
        {
            foreach (var endpoint in controller.Endpoints)
            {
                if (endpoint.ReturnType != null)
                {
                    var model = endpoint.ReturnType;

                    if (!models.Any(x => x.Name == model.Name) && !CodeGenerationUtilities.IsNative(new TypeInfo(model.Type).Type))
                    {
                        Log.Information("Adding model to the list of models that will have a .ts file (from return types): {model}", model.Name);
                        models.Add(model);

                        if (model.Name == "BaseResponse") Debugger.Break();
                    }
                    index++;
                }

                if (endpoint.InputType != null)
                {
                    var model = endpoint.InputType;

                    if (!models.Any(x => x.Name == model.Name) && !CodeGenerationUtilities.IsNative(new TypeInfo(model.Type).Type))
                    {
                        Log.Information("Adding model to the list of models that will have a .ts file (from return types): {model}", model.Name);
                        models.Add(model);

                        if (model.Name == "BaseResponse") Debugger.Break();
                    }
                    index++;
                }
            }
        }  

        // Recursively include other used models for ts code generation
        Log.Information("Recursivelly finding all other associated models");
        while (true)
        {
            var foundNewModel = false;

            for (int i = 0; i < models.Count; i++)
            {
                var model = models[i];

                Log.Information("  Analysing model: {model}", model.Name);

                foreach (var property in model.Type.GetProperties())
                {
                    var propertyTypeInfo = new TypeInfo(property.PropertyType);

                    var isNative = CodeGenerationUtilities.IsNative(propertyTypeInfo.Type);

                    if (!isNative)
                    {
                        if (!models.Any(x => x.Name == propertyTypeInfo.Name))
                        {
                            Log.Information("Adding model to the list of models that will have a .ts file (associated from {parent}): {model}", model.Name, propertyTypeInfo.Name);

                            foundNewModel = true;
                            models.Add(propertyTypeInfo);

                            if (propertyTypeInfo.Name == "BaseResponse") Debugger.Break();
                        }
                    }
                }
            }

            if (!foundNewModel)
            {
                break;
            }
        }

        var frontendModel = new FrontendModel(controllers, models.ToArray(), _basePath, _localization);

        frontendModel.GenerateModels();
        frontendModel.GenerateServices();
        frontendModel.GenerateStores();
    } 

    private List<TypeInfo> GetForcedTypescriptModels(string projectId)
    {
        var loadedAssemblies = GetRevelantAssemblies();

        var models = new List<TypeInfo>();

        foreach (var assembly in loadedAssemblies.Where(x => !x.FullName.Contains("Microsoft.Data.SqlClient")))
        {
            if (assembly.GetName().Name == "Microsoft.IdentityModel.Protocols.OpenIdConnect") continue;

            var modelTypes = (assembly.GetTypes()
                .Where(myType => ((myType.IsClass && !myType.IsAbstract) || myType.IsEnum)
                        && myType.HasAttribute<ForceTypescriptModelGenerationAttribute>()))
                .ToArray();

            foreach (var modelType in modelTypes)
            {
                var attribute = modelType.GetAttribute<ForceTypescriptModelGenerationAttribute>();

                var include = attribute.Scopes.Length == 0 || attribute.Scopes.Any(x => x == projectId);

                if (include)
                {
                    models.Add(new TypeInfo(modelType));

                    if (modelType.Name == "BaseResponse") Debugger.Break();
                }
            }
        }

        return models;
    }

    private string GetTypeScriptPropertyType(Type propertyType, out bool isExternalReference)
    {
        isExternalReference = false;

        if (propertyType == typeof(string)) return "string";
        if (propertyType == typeof(Guid)) return "string";
        if (propertyType == typeof(Boolean)) return "boolean";
        if (propertyType == typeof(DateTime)) return "Date";
        if (propertyType == typeof(Single)) return "number";
        if (propertyType == typeof(Double)) return "number";
        if (propertyType == typeof(Decimal)) return "number";
        if (propertyType == typeof(Int16)) return "number";
        if (propertyType == typeof(Int32)) return "number";
        if (propertyType == typeof(Int64)) return "number";
        if (propertyType == typeof(Object)) return "unknown";

        isExternalReference = true;

        if (propertyType.IsAssignableFrom(typeof(BaseDataTransferObject))) return propertyType.Name;

        return propertyType.Name;
    }

    private ControllerInfo[] GetControllers(string projectId, Assembly[] assemblies)
    {
        var result = new StringBuilder();

        var controllers = new List<ControllerInfo>();

        if (assemblies == null)
        {
            assemblies = GetRevelantAssemblies();
        }

        foreach (var assembly in assemblies.Where(x => !x.FullName.Contains("Microsoft.Data.SqlClient")))
        {
            var assemblyControllers = (assembly.GetTypes()
                .Where(myType => myType.IsClass
                        && !myType.IsAbstract
                        && !myType.Name.StartsWith("Base")
                        && myType.IsSubclassOf(typeof(ControllerBase)))
                .Select(x => new ControllerInfo(x, projectId)));

            foreach (var controller in assemblyControllers)
            {
                if (controller.Type.GetCodeGenerationIgnoreMode() == IgnoreMode.All)
                {
                    continue;
                }

                if (!String.IsNullOrEmpty(_projectId))
                {
                    if (controller.Scopes.Length > 0)
                    {
                        var include = controller.Scopes.Any(x => x == projectId);

                        if (!include)
                        {
                            continue;
                        }
                    }
                }

                var actionsToIgnore = new List<EndpointInfo>();

                var test1 = !String.IsNullOrEmpty(projectId);
                var test2 = CodeGenerationModuleOptions.Instance.IgnoreOptions.ContainsKey("*");

                if (!String.IsNullOrEmpty(projectId) || CodeGenerationModuleOptions.Instance.IgnoreOptions.ContainsKey("*"))
                {
                    foreach (var action in controller.Endpoints)
                    {
                        var patterns = new List<string>();

                        if (!String.IsNullOrEmpty(projectId))
                        {
                            CodeGenerationModuleOptions.Instance.IgnoreOptions.TryGetValue(projectId, out patterns);
                        }

                        if (patterns == null)
                        {
                            patterns = new List<string>();
                        }

                        CodeGenerationModuleOptions.Instance.IgnoreOptions.TryGetValue("*", out var patterns2);

                        if (patterns2 != null)
                        {
                            patterns.AddRange(patterns2);
                        }

                        foreach (var pattern in patterns)
                        {
                            var actionRoute = String.IsNullOrEmpty(action.Route) ? "" : ("/" + action.Route);
                            var completeRoute1 = $"{action.Method.ToString().ToUpper()} {controller.Route}{action.Route}";
                            var completeRoute2 = $"* {controller.Route}{action.Route}";

                            if (completeRoute1.ToLower().StartsWith(pattern.ToLower()) ||
                                completeRoute2.ToLower().StartsWith(pattern.ToLower()))
                            {
                                actionsToIgnore.Add(action);
                            }
                        }
                    }
                }

                foreach (var action in actionsToIgnore)
                {
                    controller.Endpoints.Remove(action);
                }

                if (controller.Endpoints.Count > 0)
                {
                    controllers.Add(controller);
                }
            }
        }

        return controllers.ToArray();
    }

    private Assembly[] GetRevelantAssemblies()
    {
        AppDomain currentDomain = AppDomain.CurrentDomain;
        var assemblies = currentDomain.GetAssemblies();

        var loadedAssemblies = new List<Assembly>();

        foreach (var assembly in assemblies)
        {
            if (!assembly.FullName.StartsWith("Microsoft.") &&
                !assembly.FullName.StartsWith("AutoMapper") &&
                !assembly.FullName.StartsWith("netstandard") &&
                !assembly.FullName.StartsWith("Swashbuckle.") &&
                !assembly.FullName.StartsWith("MediatR") &&
                !assembly.FullName.StartsWith("FluentValidation") &&
                !assembly.FullName.StartsWith("MediatR.") &&
                !assembly.FullName.StartsWith("mscorlib") &&
                !assembly.FullName.StartsWith("Anonymously") &&
                !assembly.FullName.StartsWith("System."))
            {
                loadedAssemblies.Add(assembly);
            }
        }

        return assemblies.ToArray();
    }
}