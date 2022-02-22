using System;

namespace rbkApiModules.CodeGeneration.Commons
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ForceTypescriptModelGenerationAttribute : Attribute
    {
        public ForceTypescriptModelGenerationAttribute(params string[] scopes)
        {
            if (scopes != null)
            {
                Scopes = scopes;
            }
            else
            {
                Scopes = new string[0];
            }
        }

        public string[] Scopes { get; set; }
    }
}
