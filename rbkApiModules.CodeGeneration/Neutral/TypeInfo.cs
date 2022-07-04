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
    public class TypeInfo
    {
        public TypeInfo(Type type)
        {
            // if (type.FullName.Contains("Tree")) Debugger.Break();

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

            if (Type.Name == typeof(SimpleNamedEntity<>).Name && Type.GenericTypeArguments.First() == typeof(Int32))
            {
                Name = "{ id: number, name: string }";
            }
            if (Type.Name == typeof(SimpleNamedEntity<>).Name && Type.GenericTypeArguments.First() != typeof(Int32))
            {
                Name = "{ id: string, name: string }";
            }
            else if (Type != typeof(Object) && (Type.IsAssignableFrom(typeof(TreeNode))
                || Type.BaseType == typeof(TreeNode)
                || (Type.BaseType.IsGenericType && Type.BaseType.GetGenericTypeDefinition() == typeof(TreeNode<>))))
            {
                Name = "TreeNode";
            }
            else
            {
                Name = Type.FullName.Split('.').Last().Replace("[]", "").Replace("+Command", "").Replace("+", "");
            }
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
}