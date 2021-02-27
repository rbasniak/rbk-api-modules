//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Xml;

//namespace rbkApiModules.CodeGenerator
//{
//    public static class Test
//    {
//        public List<ApiMethod> GetApiMethods()
//        {
//            /*--- Inits ---*/
//            var result = new List<ApiMethod>();

//            var asm = Assembly.Load(assemblyName);

//            var xmlFile = Path.Combine(Path.GetDirectoryName(asm.Location), Path.GetFileNameWithoutExtension(asm.Location)) + ".xml";

//            var xmlDoc = new XmlDocument();

//            xmlDoc.Load(xmlFile);

//            /*--- API Controllers ---*/
//            var apiControllers = asm.DefinedTypes.Where(a => a.BaseType == typeof(ApiController));

//            foreach (var c in apiControllers)
//            {
//                var rpAttrib = c.GetCustomAttributes(typeof(RoutePrefixAttribute)).FirstOrDefault() as RoutePrefixAttribute;
//                var routePrefix = rpAttrib?.Prefix ?? string.Empty;

//                var methods = c.GetMethods();

//                foreach (var m in methods)
//                {
//                    /*--- Skip this method? ---*/
//                    if (!m.IsPublic || m.IsSpecialName || !m.Module.Name.StartsWith(AssemblyName))
//                        continue;

//                    /*--- Determine HTTP Verb ---*/
//                    var getAttrib = m.GetCustomAttributes(typeof(HttpGetAttribute)).FirstOrDefault() as HttpGetAttribute;
//                    var postAttrib = m.GetCustomAttributes(typeof(HttpPostAttribute)).FirstOrDefault() as HttpPostAttribute;
//                    var putAttrib = m.GetCustomAttributes(typeof(HttpPutAttribute)).FirstOrDefault() as HttpPutAttribute;
//                    var deleteAttrib = m.GetCustomAttributes(typeof(HttpDeleteAttribute)).FirstOrDefault() as HttpDeleteAttribute;
//                    var acceptVerbsAttrib = m.GetCustomAttributes(typeof(AcceptVerbsAttribute)).FirstOrDefault() as AcceptVerbsAttribute;

//                    var v = acceptVerbsAttrib == null || acceptVerbsAttrib.HttpMethods.Count == 0
//                        ? HttpVerb.GET
//                        : (HttpVerb)Enum.Parse(typeof(HttpVerb), acceptVerbsAttrib.HttpMethods[0].Method.ToUpperInvariant());

//                    var httpVerb = getAttrib != null
//                        ? HttpVerb.GET
//                        : postAttrib != null
//                            ? HttpVerb.POST
//                            : putAttrib != null
//                                ? HttpVerb.PUT
//                                : deleteAttrib != null
//                                    ? HttpVerb.DELETE
//                                    : v;

//                    /*--- Determine Route ---*/
//                    var routeAttribs = m.GetCustomAttributes(typeof(RouteAttribute));

//                    var routeSuffixes = routeAttribs.Select(a => (a as RouteAttribute)?.Template).Where(a => !string.IsNullOrWhiteSpace(a)).ToList();

//                    if (!routeSuffixes.Any())
//                        routeSuffixes.Add(m.Name);

//                    var parameters = m.GetParameters();

//                    var signature = string.Join(", ", parameters.Select(a => a.ParameterType.Name).ToArray<string>());

//                    /*--- Retrieve Summary from XML Comments ---*/
//                    var methodFullName = $"M:{c.FullName}.{m.Name}";

//                    if (parameters.Length > 0)
//                    {
//                        methodFullName += "(";
//                        methodFullName += string.Join(",", parameters.Select(a => a.ParameterType.FullName).ToArray<string>());
//                        methodFullName += ")";
//                    }

//                    var xPath = $"/doc/members/member[@name='{methodFullName}']/summary";

//                    var node = xmlDoc.SelectSingleNode(xPath);

//                    var summary = CleanXml(node);

//                    /*--- Loop Cleanup ---*/
//                    var apiMethod = new ApiMethod
//                    {
//                        Routes = routeSuffixes.Select(a => routePrefix + (routePrefix.Length == 0 ? string.Empty : "/") + a).ToList(),
//                        ControllerName = c.Name,
//                        MethodName = m.Name,
//                        ParameterList = signature,
//                        HttpVerb = httpVerb,
//                        Summary = summary
//                    };

//                    result.Add(apiMethod);
//                }
//            }

//            /*--- ODATA Controllers ---*/
//            /* Since we have only 7 ODATA endpoints as of this writing, we'll do these manually. */

//            /*--- Clean Up ---*/
//            return result;
//        }

//        private static string CleanXml(XmlNode node)
//        {
//            var result = node?.InnerXml
//                                .Replace(@"\r", "")
//                                .Replace(@"\n", "")
//                                .Trim()
//                                .Replace("&", "&amp;")
//                                .Replace("'", "&quot;")
//                                ?? string.Empty;

//            var len = 0;

//            while (result.Length != len)
//            {
//                len = result.Length;
//                result = result.Replace("  ", " ");
//            }

//            return result;
//        }

//        public enum HttpVerb
//        {
//            GET,
//            POST,
//            PUT,
//            DELETE,
//            PATCH
//        }

//        public class ApiMethod
//        {
//            public List<string> Routes { get; set; }
//            public string ControllerName { get; set; }
//            public string MethodName { get; set; }
//            public string ParameterList { get; set; }
//            public HttpVerb HttpVerb { get; set; }
//            public string Summary { get; set; }
//        }
//    }
//}