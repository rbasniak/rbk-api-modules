
namespace rbkApiModules.Testing.Core;

public class HumanFriendlyDisplayNameAttribute : DisplayNameFormatterAttribute
{
    protected override string FormatDisplayName(DiscoveredTestContext context)
    {
        try
        {
            int order = 0;

            if (context.TestDetails.AttributesByType.ContainsKey(typeof(NotInParallelAttribute)))
            {
                var attribute = (NotInParallelAttribute)context.TestDetails.AttributesByType[typeof(NotInParallelAttribute)].First();
                order = attribute.Order;
                return $"T{order:000}: {context.TestContext.Metadata.TestDetails.TestName.Replace("_", " ")}";
            }
            else
            {
                return $"{context.TestContext.Metadata.TestDetails.TestName.Replace("_", " ")}";
            }

        }
        catch (Exception ex)
        {
            return context.TestContext.Metadata.TestDetails.TestName + " (FAIL TO COMPUTE NAME)";
        }
    }
}