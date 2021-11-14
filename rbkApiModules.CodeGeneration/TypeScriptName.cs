using System;

namespace rbkApiModules.CodeGeneration.Commons
{
    public class TypeScriptName : Attribute
    {
        public TypeScriptName(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
