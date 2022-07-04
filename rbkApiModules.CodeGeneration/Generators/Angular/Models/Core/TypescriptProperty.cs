using rbkApiModules.CodeGeneration.Commons;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.CodeGeneration
{
    public class TypescriptProperty
    {
        public TypescriptProperty(string name, TypeInfo info, bool isObservable)
        {
            Name = CodeGenerationUtilities.ToCamelCase(name);
            IsArray = info.IsList;
            IsOptional = info.Nullable;
            IsObservable = isObservable;
            Type = new TypescriptType(info);

            HasDateProperty = info.Type.HasDateProperty();

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
        public bool HasDateProperty { get; set; }

        public override string ToString()
        {
            return Declaration;
        }
    }
}
