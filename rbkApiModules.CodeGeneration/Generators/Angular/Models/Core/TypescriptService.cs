using rbkApiModules.CodeGeneration.Commons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.CodeGeneration
{
    public class TypescriptService
    {
        public TypescriptService(ControllerInfo controller)
        {
            Name = controller.Name + "Service";

            Filename = $"{CodeGenerationUtilities.ToTypeScriptFileCase(controller.Name) + ".service"}";
            Filepath = Path.Join($"services", "api", $"{Filename}.ts");
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
            code.AppendLine("import { BaseApiService } from 'ngx-smz-ui';");
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

                var terminator = ";";

                if (method.ReturnType != null && method.ReturnType.HasDateProperty)
                {
                    externalReferences.Add("import { fixDates } from 'ngx-smz-ui';");
                    terminator = ".pipe(";
                }


                if (method.Method.ToUpper() == "GET" || method.Method.ToUpper() == "DELETE")
                {
                    if (method.Parameters.Count == 0)
                    {
                        code.AppendLine($"    return this.http.{httpMethod}<{returnType}>(`${{this.endpoint}}{route}`, this.generateDefaultHeaders({{}}))" + terminator);

                        if (method.ReturnType != null && method.ReturnType.HasDateProperty)
                        {
                            code.AppendLine($"      fixDates()");
                            code.AppendLine($"    );");
                        }
                    }
                    else if (method.Parameters.Count == 1)
                    {
                        code.AppendLine($"    return this.http.{httpMethod}<{returnType}>(`${{this.endpoint}}{route.Replace("{", "${")}`, this.generateDefaultHeaders({{}}))" + terminator);

                        if (method.ReturnType != null && method.ReturnType.HasDateProperty)
                        {
                            code.AppendLine($"      fixDates()");
                            code.AppendLine($"    );");
                        }
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
                        code.AppendLine($"    return this.http.{httpMethod}<{returnType}>(`${{this.endpoint}}{route}`, null, this.generateDefaultHeaders({{}}))" + terminator);

                        if (method.ReturnType != null && method.ReturnType.HasDateProperty)
                        {
                            code.AppendLine($"      fixDates()");
                            code.AppendLine($"    );");
                        }
                    }
                    else if (method.Parameters.Count == 1)
                    {
                        code.AppendLine($"    return this.http.{httpMethod}<{returnType}>(`${{this.endpoint}}{route}`, {method.Parameters.First().Name}, this.generateDefaultHeaders({{}}))" + terminator);

                        if (method.ReturnType != null && method.ReturnType.HasDateProperty)
                        {
                            code.AppendLine($"      fixDates()");
                            code.AppendLine($"    );");
                        }
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
}
