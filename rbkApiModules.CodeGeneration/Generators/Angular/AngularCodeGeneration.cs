using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using rbkApiModules.CodeGeneration.Commons;
using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.CodeGeneration
{
    public class AngularCodeGenerator
    {
        private string _basePath = "";
        private string _projectId = "";

        public AngularCodeGenerator(string projectId, string basePath)
        {
            _basePath = basePath;
            _projectId = projectId;
        }

        public void Generate()
        {
            var servicesFolder = Path.Combine(_basePath, "services", "api");
            Directory.CreateDirectory(servicesFolder);

            var controllers = GetControllers(_projectId);

            var models = GetForcedTypescriptModels(_projectId);

            var index = 0;

            // Select the 1st level models (inputs for endpoints)
            foreach (var model in controllers.SelectMany(x => x.Endpoints).Where(x => x.ReturnType != null).Select(x => x.ReturnType))
            {
                if (!models.Any(x => x.Name == model.Name) && !CodeGenerationUtilities.IsNative(new TypeInfo(model.Type).Type))
                {
                    models.Add(model);
                }
                index++;
            }

            // Select the 1st level models (return from endpoints)
            foreach (var model in controllers.SelectMany(x => x.Endpoints).Where(x => x.InputType != null).Select(x => x.InputType))
            {
                if (!models.Any(x => x.Name == model.Name) && !CodeGenerationUtilities.IsNative(new TypeInfo(model.Type).Type))
                {
                    models.Add(model);
                }
            }

            // Recursively include other used models for ts code generation
            while (true)
            {
                var foundNewModel = false;

                for (int i = 0; i < models.Count; i++)
                {
                    var model = models[i];
                    foreach (var property in model.Type.GetProperties())
                    {
                        var propertyTypeInfo = new TypeInfo(property.PropertyType);

                        if (!CodeGenerationUtilities.IsNative(propertyTypeInfo.Type))
                        {
                            if (!models.Any(x => x.Name == propertyTypeInfo.Name))
                            {
                                foundNewModel = true;
                                models.Add(propertyTypeInfo);
                            }
                        }
                    }
                }

                if (!foundNewModel)
                {
                    break;
                }
            }

            var frontendModel = new FrontendModel(controllers, models.ToArray(), _basePath);

            frontendModel.GenerateModels();
            frontendModel.GenerateServices();
            frontendModel.GenerateStores();
        }

        private List<TypeInfo> GetForcedTypescriptModels(string projectId)
        {
            var loadedAssemblies = GetRevelantAssemblies();

            var models = new List<TypeInfo>();

            foreach (var assembly in loadedAssemblies)
            {
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

        private ControllerInfo[] GetControllers(string projectId)
        {
            var result = new StringBuilder();

            var loadedAssemblies = GetRevelantAssemblies();

            var controllers = new List<ControllerInfo>();

            foreach (var assembly in loadedAssemblies)
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

                    if (!String.IsNullOrEmpty(projectId))
                    {
                        foreach (var action in controller.Endpoints)
                        {
                            if (CodeGenerationModuleOptions.Instance.IgnoreOptions.TryGetValue(projectId, out var patterns))
                            {
                                foreach (var pattern in patterns)
                                {
                                    var actionRoute = String.IsNullOrEmpty(action.Route) ? "" : ("/" + action.Route);
                                    var completeRoute1 = $"{action.Method.ToString().ToUpper()} {controller.Route}{action.Route}";
                                    var completeRoute2 = $"* {controller.Route}{action.Route}";

                                    if (completeRoute1.ToLower().StartsWith(pattern.ToLower()) || 
                                        completeRoute2.ToLower().StartsWith(pattern.ToLower()) ||
                                        pattern == "*")
                                    {
                                        actionsToIgnore.Add(action);
                                    }
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
}