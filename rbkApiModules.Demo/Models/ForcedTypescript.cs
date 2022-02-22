using rbkApiModules.CodeGeneration.Commons;

namespace rbkApiModules.Demo.Models
{
    [ForceTypescriptModelGeneration("project-a", "project-c")]
    public class ForcedTypescriptModel
    {
        public int MyProperty1 { get; set; }
        public bool MyProperty2 { get; set; }
        public string MyProperty3 { get; set; }
        public SubForcedModel MyProperty4 { get; set; }
    }

    public class SubForcedModel
    {
        public int MyProperty { get; set; }
    }
}
