using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using rbkApiModules.CodeGeneration.Commons;
using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
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

        public AngularCodeGenerator(string basePath)
        {
            _basePath = basePath;
        }

        public object GetData()
        {
            var servicesFolder = Path.Combine(_basePath, "services", "api");
            Directory.CreateDirectory(servicesFolder);

            var controllers = GetControllers();

            var models = new List<TypeInfo>();

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

            var frontendModel = new AngularModel(controllers, models.ToArray(), _basePath);

            frontendModel.GenerateModels();
            frontendModel.GenerateServices();

            //foreach (var endpoint in controllers.SelectMany(x => x.Endpoints).Where(x => x.HasDatabaseState))
            //{
            //    switch (endpoint.DatabaseStateType)
            //    {
            //        case StoreType.Readonly:
            //            // GenerateDatabase(endpoint, dbStatesFolder);
            //            break;
            //        default:
            //            throw new Exception();
            //    }
            //} 

            return new { Controllers = controllers, Models = models.Select(x => x.Name) };
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

        private ControllerInfo[] GetControllers()
        {
            var result = new StringBuilder();

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

            var controllers = new List<ControllerInfo>();
            foreach (var assembly in loadedAssemblies)
            {
                controllers.AddRange(assembly.GetTypes()
                    .Where(myType => myType.IsClass
                            && !myType.IsAbstract
                            && !myType.Name.StartsWith("Base")
                            && myType.IsSubclassOf(typeof(ControllerBase))
                            && !myType.HasAttribute<IgnoreOnCodeGenerationAttribute>())
                    .Select(x => new ControllerInfo(x)));
            }

            return controllers.ToArray();
        }
    }

    public class EndpointInfo
    {
        public EndpointInfo(ControllerInfo controller, MethodInfo action)
        {
            Name = action.Name;

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
                if (parameter.ParameterType.FullName.EndsWith("+Command"))
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

            //if (action.HasAttribute<NgxsDatabaseStoreAttribute>())
            //{
            //    var attribute = action.GetAttribute<NgxsDatabaseStoreAttribute>();

            //    DatabaseStateType = attribute.Type;
            //}
        }

        public HttpMethod Method { get; set; }
        public string Name { get; set; }
        public string Route { get; set; }
        public List<PropertyInfo> UrlParameters { get; set; }
        public TypeInfo InputType { get; set; }
        public TypeInfo ReturnType { get; set; }
        // public StoreType? DatabaseStateType { get; set; }
        //public bool HasDatabaseState => DatabaseStateType != null;
    }

    public class TypeInfo
    {
        public TypeInfo(Type type)
        {
            if (type.IsList())
            {
                IsList = true;
                Type = type.GetInterfaces().Single(x => x.Name == typeof(IEnumerable<>).Name).GenericTypeArguments.First();
            }
            else
            {
                Type = type;
            }

            if (Type.Name == typeof(Nullable<>).Name)
            {
                Nullable = true;
                Type = Type.GenericTypeArguments.First();
            }

            Name = Type.FullName.Split('.').Last().Replace("[]", "").Replace("+Command", "").Replace("+", "");
        }

        public Type Type { get; set; }
        public bool IsList { get; set; }
        public bool Nullable { get; set; }
        public string Name { get; set; }
        public List<PropertyInfo> Properties { get; set; }

        public override string ToString()
        {
            return Name + (IsList ? "[]" : "");
        }
    }

    public class PropertyInfo
    {
        public PropertyInfo(System.Reflection.PropertyInfo propertyInfo)
        {
            Name = propertyInfo.Name;
            Type = new TypeInfo(propertyInfo.PropertyType);
        }

        public PropertyInfo(string name, Type type)
        {
            Name = name;
            Type = new TypeInfo(type);
        }

        public string Name { get; set; }
        public TypeInfo Type { get; set; }
    }

    public enum HttpMethod
    {
        Get,
        Post,
        Put,
        Delete
    }

    public class ControllerInfo
    {
        public ControllerInfo(Type type)
        {
            Type = type;

            Name = type.FullName.Split('.').Last().Replace("Controller", String.Empty);
            var routeAttribute = type.GetAttribute<RouteAttribute>();
            Route = routeAttribute != null ? routeAttribute.Template.Replace("[controller]", Name.ToLower()) : String.Empty;

            if (type.HasAttribute<NgxsDatabaseStoreAttribute>())
            {
                var attribute = type.GetAttribute<NgxsDatabaseStoreAttribute>();

                StoreType = attribute.Type;
            }

            Endpoints = GetEndpoints(type);
        }

        public Type Type { get; private set; }
        public string Name { get; private set; }
        public string Route { get; private set; }
        public EndpointInfo[] Endpoints { get; private set; }
        public StoreType? StoreType { get; private set; }

        private EndpointInfo[] GetEndpoints(Type controllerType)
        {
            var methods = controllerType.GetMethods()
                .Where(x => x.GetCustomAttributes().Any(x => x.GetType().IsSubclassOf(typeof(HttpMethodAttribute))) && !x.HasAttribute<IgnoreOnCodeGenerationAttribute>());

            var results = new List<EndpointInfo>();

            foreach (var action in methods)
            {
                results.Add(new EndpointInfo(this, action));
            }

            return results.ToArray();
        }
    }

    public class AngularModel
    {
        private readonly string _basePath;
        private readonly string _modelsPath;

        public AngularModel(ControllerInfo[] controllers, TypeInfo[] models, string basePath)
        {
            _basePath = basePath;

            _modelsPath = Path.Combine(basePath, "models");

            Services = new List<TypescriptService>();
            Models = new List<TypescriptModel>();
            Stores = new List<NgxsStore>();

            foreach (var model in models)
            {
                Models.Add(new TypescriptModel(model));
            }

            foreach (var controller in controllers)
            {
                var service = new TypescriptService(controller);
                Services.Add(service);
                // Stores.Add(new NgxsStore(service));
            }
        }

        public List<TypescriptService> Services { get; set; }
        public List<TypescriptModel> Models { get; set; }
        public List<NgxsStore> Stores { get; set; }

        public TypescriptModel GetModel(string name)
        {
            return Models.Single(x => x.Name == name);
        }

        public void GenerateModels()
        {
            foreach (var model in Models.Where(x => x.Filepath != null))
            {
                var filename = Path.Combine(_basePath, model.Filepath);
                Directory.CreateDirectory(Path.GetDirectoryName(filename));

                File.WriteAllText(filename, model.GenerateCode(Models));
            }
        }

        public void GenerateServices()
        {
            foreach (var service in Services)
            {
                var filename = Path.Combine(_basePath, service.Filepath);
                Directory.CreateDirectory(Path.GetDirectoryName(filename));

                File.WriteAllText(filename, service.GenerateCode(Models));
            }
        }
    }

    public class NgxsStore
    {
        public NgxsStore CreateReadonly(TypescriptService service)
        {
            var store = new NgxsStore();
            store.Name = service.Name;
            // Actions.Actions.Add(new NgxsAction(service.Methods.First());



            return store;
        }

        public NgxsStore()
        {
            Actions = new NgxsActionFile();
            Selectors = new NgxsSelectorFile();
            State = new NgxsState();
        }

        public NgxsStore(ControllerInfo controller)
        {
            if (controller.StoreType == StoreType.Complete)
            {

            }

            if (controller.StoreType == StoreType.Readonly)
            {
                Name = controller.Name;

                if (controller.Endpoints.Length != 1) throw new Exception("Readonly stores can have only one endpoint");

                Actions = new NgxsActionFile();
                Actions.Name = Name;
                Actions.Actions.Add(new NgxsAction("LoadAll", $"{controller.Name} API", controller.Endpoints.First()));

                Selectors = new NgxsSelectorFile();
                Selectors.Name = Name;
                // Selectors.Selectors.Add(new NgxsSelector("all"));
            }
        }

        public string Name { get; set; }
        public NgxsState State { get; set; }
        public NgxsActionFile Actions { get; set; }
        public NgxsSelectorFile Selectors { get; set; }
    }

    public class NgxsSelectorFile
    {
        public string Name { get; set; }
        public List<NgxsSelector> Selectors { get; set; }

        public string GenerateCode()
        {
            var code = $@"
import {{ Selector }} from '@ngxs/store';
// import {{ UnsDbState, UnsDbStateModel }} from './uns.state';
// import {{ UnListItem }} from '@models/domain/un';

export class {Name}Selectors {{
  SELECTORS_HERE
}}
";

            return code;
        }
    }

    public class NgxsSelector : TypescriptMethod
    {
        public NgxsSelector(string name, TypescriptProperty returnType, List<TypescriptProperty> parameters)
        {

        }

        public NgxsState StateModel { get; set; }

        public string GenerateCode()
        {
            //            var code = $@"
            //  @Selector([{StateModel.FinalName}])
            //  public static {Name}(state: {StateModel.ModelName}): {ReturnType}[] {{
            //    return cloneDeep(state.items);
            //  }}
            //";

            //            return code;
            return "";
        }
    }

    public class NgxsActionFile
    {
        public string Name { get; set; }
        public List<NgxsAction> Actions { get; set; }
    }

    public class NgxsState
    {
        public TypescriptModel StateModel { get; set; }
        public NgxsStore Parent { get; set; }
        public TypescriptService InjectedServices { get; set; }
        public List<NgxsHandler> Handlers { get; set; }
    }

    public class NgxsHandler
    {
        private TypescriptServiceMethod _typescriptServiceMethod;
        public NgxsHandler(NgxsAction action, TypescriptServiceMethod method)
        {
            _typescriptServiceMethod = method;
            Action = action;
        }

        public NgxsAction Action { get; set; }
        public TypescriptMethod Method { get; set; }
    }

    public class NgxsAction
    {
        public NgxsAction(string name, string domain, TypescriptServiceMethod method, bool hasSuccess = false)
        {
            Name = name;
            Domain = domain;
            HasSuccess = hasSuccess;
            Parameters = new List<TypescriptProperty>();
            ReturnType = method.ReturnType;
        }

        public NgxsAction(string name, string domain, EndpointInfo endpoint, bool hasSuccess = false)
        {
            Name = name;
            Domain = domain;
            HasSuccess = hasSuccess;
            Parameters = new List<TypescriptProperty>();
            if (endpoint.InputType != null)
            {
                Parameters.Add(new TypescriptProperty("data", endpoint.InputType, false));
            }
        }

        public string Name { get; set; }
        public bool HasSuccess { get; set; }
        public string Domain { get; set; }
        public TypescriptProperty ReturnType { get; set; }
        public List<TypescriptProperty> Parameters { get; set; }

        public string GenerateCode()
        {
            var code = $@"
  export class ${Name} {{
    public static readonly type = '[{Domain}] {Name}'
  }}
";
            return code;
        }
    }

    public class TypescriptService
    {
        public TypescriptService(ControllerInfo controller)
        {
            Name = controller.Name + "Service";

            Filename = $"{CodeGenerationUtilities.ToTypeScriptFileCase(controller.Name) + ".service"}";
            Filepath = $"services\\api\\{Filename}.ts";
            ImportStatement = $"import {{ {Name} }} from '@services/api/{Filename}';";

            BaseRoute = controller.Route;

            Methods = controller.Endpoints.Select(x => new TypescriptServiceMethod(x)).ToList();
        }

        public string Name { get; set; }
        public string BaseRoute { get; set; }
        public string Filename { get; set; }
        public string Filepath { get; set; }
        public string ImportStatement { get; set; }
        public List<TypescriptServiceMethod> Methods { get; set; }

        public string GenerateCode(List<TypescriptModel> models)
        {
            var code = new StringBuilder();

            code.AppendLine("import { Injectable } from '@angular/core';");
            code.AppendLine("import { environment } from '@environments/environment';");
            code.AppendLine("import { HttpClient } from '@angular/common/http';");
            code.AppendLine("import { Observable } from 'rxjs/internal/Observable';");
            code.AppendLine("import { BaseApiService } from 'ngx-rbk-utils';");
            code.AppendLine("{{EXTERNAL_REFERENCES}}");
            code.AppendLine("@Injectable({ providedIn: 'root' })");
            code.AppendLine("export class " + Name + " extends BaseApiService {");
            code.AppendLine("  private endpoint = `${environment.serverUrl}/" + BaseRoute + "`;");
            code.AppendLine("");
            code.AppendLine("  constructor(private http: HttpClient) {");
            code.AppendLine("    super();");
            code.AppendLine("  }");
            code.AppendLine("");

            var externalReferences = new HashSet<string>();

            foreach (var method in Methods)
            {
                var inputs = "";

                foreach (var input in method.Parameters)
                {
                    if (!input.Type.IsNative)
                    {
                        externalReferences.Add(models.First(x => x.Name == input.Type.Name).ImportStatement);
                    }

                    inputs += $"{input.Declaration}";
                }

                if (method.ReturnType != null && !method.ReturnType.Type.IsNative)
                {
                    externalReferences.Add(models.First(x => x.Name == method.ReturnType.Type.Name).ImportStatement);
                }

                inputs = inputs.Trim(' ').Trim(',');

                var returnType = method.ReturnType != null ? method.ReturnType.FinalType : "void";

                code.AppendLine("  public " + method.Name + "(" + inputs + "): Observable<" + returnType + "> {");

                var route = String.IsNullOrEmpty(method.Route) ? "" : ("/" + method.Route);
                var httpMethod = method.Method.ToString().ToLower();

                if (method.Method.ToUpper() == "GET" || method.Method.ToUpper() == "DELETE")
                {
                    if (method.Parameters.Count == 0)
                    {
                        code.AppendLine($"    return this.http.{httpMethod}<{returnType}>(`${{this.endpoint}}{route}`, this.generateDefaultHeaders({{}}));");
                    }
                    else if (method.Parameters.Count == 1)
                    {
                        code.AppendLine($"    return this.http.{httpMethod}<{returnType}>(`${{this.endpoint}}{route.Replace("{", "${")}`, this.generateDefaultHeaders({{}}));");
                    }
                    else
                    {
                        throw new Exception("Endpoint with multiple parameters are not supported");
                    }
                }

                if (method.Method.ToUpper() == "POST" || method.Method.ToUpper() == "PUT")
                {
                    if (method.Parameters.Count == 0)
                    {
                        code.AppendLine($"    return this.http.{httpMethod}<{returnType}>(`${{this.endpoint}}{route}`, null, this.generateDefaultHeaders({{}}));");
                    }
                    else if (method.Parameters.Count == 1)
                    {
                        code.AppendLine($"    return this.http.{httpMethod}<{returnType}>(`${{this.endpoint}}{route}`, {method.Parameters.First().Name}, this.generateDefaultHeaders({{}}));");
                    }
                    else
                    {
                        throw new Exception("Endpoint with multiple parameters are not supported");
                    }
                }


                code.AppendLine("  }");
            }

            code.AppendLine("}");
            code.AppendLine("");

            var references = new StringBuilder();
            foreach (var reference in GetReferences())
            {
                references.AppendLine(reference.ImportStatement);
            }

            code = code.Replace("{{EXTERNAL_REFERENCES}}", String.Join(Environment.NewLine, externalReferences) + Environment.NewLine + Environment.NewLine);

            return code.ToString();
        }

        private List<TypescriptModel> GetReferences()
        {
            var references = new List<TypescriptModel>();
            //foreach (var method in Methods)
            //{
            //    if (method.ReturnType != null && !method.ReturnType.Type.IsNative)
            //    {
            //        references.Add(method.ReturnType.Type.Model);
            //    }

            //    foreach (var parameter in method.Parameters)
            //    {
            //        if (!parameter.Type.IsNative)
            //        {
            //            references.Add(parameter.Type.Model);
            //        }
            //    }
            //}

            return references;
        }
    }

    public class TypescriptServiceMethod : TypescriptMethod
    {
        public TypescriptServiceMethod(EndpointInfo endpoint)
        {
            Name = CodeGenerationUtilities.ToCamelCase(endpoint.Name);
            Route = endpoint.Route;
            Method = endpoint.Method.ToString().ToLower();

            if (endpoint.ReturnType != null)
            {
                ReturnType = new TypescriptProperty("", endpoint.ReturnType, false);
            }

            Parameters = new List<TypescriptProperty>();

            foreach (var parameter in endpoint.UrlParameters)
            {
                Parameters.Add(new TypescriptProperty(parameter.Name, parameter.Type, false));
            }

            if (endpoint.InputType != null)
            {
                Parameters.Add(new TypescriptProperty("data", endpoint.InputType, false));
            }
        }

        public string Route { get; set; }
        public string Method { get; set; }
    }

    public class TypescriptMethod
    {
        public TypescriptMethod()
        {

        }

        public string Name { get; set; }
        public TypescriptProperty ReturnType { get; set; }
        public List<TypescriptProperty> Parameters { get; set; }
    }

    public class TypescriptModel
    {
        public TypescriptModel(TypeInfo type)
        {
            Name = type.Name;
            Filename = CodeGenerationUtilities.ToTypeScriptFileCase(type.Name);

            if (type.Name == "SimpleNamedEntity")
            {
                Filepath = null;
                ImportStatement = $"import {{ {type.Name} }} from 'ngx-smz-dialogs';";
            }
            else
            {
                Filepath = $"models\\{CodeGenerationUtilities.ToTypeScriptFileCase(type.Name)}.ts";
                ImportStatement = $"import {{ {type.Name} }} from '@models/{Filename}';";
            }


            Properties = new List<TypescriptProperty>();

            foreach (var property in type.Type.GetProperties())
            {
                Properties.Add(new TypescriptProperty(property.Name, new TypeInfo(property.PropertyType), false));
            }
        }

        public string Name { get; set; }
        public List<TypescriptProperty> Properties { get; private set; }
        public string Filename { get; private set; }
        public string Filepath { get; private set; }
        public string ImportStatement { get; private set; }

        public string GenerateCode(List<TypescriptModel> models)
        {
            var code = new StringBuilder();

            var externalReferences = new HashSet<string>();
            code.Append("{{EXTERNAL_REFERENCES}}");
            code.Append($"export interface {Name}" + " {" + Environment.NewLine);

            foreach (var property in Properties)
            {
                var optional = property.IsOptional ? "?" : "";
                code.AppendLine($"  {property.Declaration};");

                if (!property.Type.IsNative && property.Name != Name)
                {
                    externalReferences.Add(models.First(x => x.Name == property.Type.Name).ImportStatement);
                }
            }

            code.AppendLine("}");

            code = code.Replace("{{EXTERNAL_REFERENCES}}", String.Join(Environment.NewLine, externalReferences) + Environment.NewLine + Environment.NewLine);

            return code.ToString();
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class TypescriptProperty
    {
        public TypescriptProperty(string name, TypeInfo info, bool isObservable)
        {
            Name = CodeGenerationUtilities.ToCamelCase(name);
            IsArray = info.IsList;
            IsOptional = info.Nullable;
            IsObservable = isObservable;
            Type = new TypescriptType(info.Name);

            var arrayModifier = IsArray ? "[]" : "";
            var optionalModifier = IsOptional ? "?" : "";

            if (IsObservable)
            {
                Declaration = $"Observable<{Declaration}>";
            }

            FinalType = $"{Type}{arrayModifier}";

            Declaration = $"{Name}{optionalModifier}: {FinalType}";
        }

        public string Name { get; set; }
        public TypescriptType Type { get; set; }
        public bool IsArray { get; set; }
        public bool IsOptional { get; set; }
        public bool IsObservable { get; set; }
        public string Declaration { get; set; }
        public string FinalType { get; set; }

        public override string ToString()
        {
            return Declaration;
        }
    }

    public class TypescriptType
    {
        public TypescriptType(string type)
        {
            IsNative = true;

            switch (type)
            {
                case "String":
                    Name = "string";
                    break;
                case "Guid":
                    Name = "string";
                    break;
                case "Boolean":
                    Name = "boolean";
                    break;
                case "DateTime":
                    Name = "Date";
                    break;
                case "Single":
                case "Double":
                case "Decimal":
                case "Int16":
                case "Int32":
                case "Int64":
                    Name = "number";
                    break;
                case "Object":
                    Name = "unknown";
                    break;
                default:
                    Name = type;
                    IsNative = false;
                    break;
            }
        }

        public string Name { get; set; }
        public bool IsNative { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

}
