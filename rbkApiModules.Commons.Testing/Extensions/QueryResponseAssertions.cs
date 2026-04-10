using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Commons.Core;
using Shouldly;

namespace rbkApiModules.Testing.Core;

public static class QueryResponseAssertions
{
    public static void ShouldHaveMessages<T>(this QueryResponse<T> response, params string[] expectedMessages)
    {
        response.Error.ShouldBeAssignableTo<ValidationProblemDetails>();

        if (response.Error is ValidationProblemDetails validationDetails)
        {
            var actualMessages = validationDetails.Errors?
                .SelectMany(kvp => kvp.Value)
                .ToArray() ?? Array.Empty<string>();

            actualMessages.Count().ShouldBe(expectedMessages.Length, 
                $"Expected {expectedMessages.Length} messages, but found {actualMessages.Length}.\n" +
                $"Actual messages: [{string.Join(", ", actualMessages)}]\n" +
                $"Expected messages: [{string.Join(", ", expectedMessages)}]");

            // Check if all expected messages are present in the aggregated messages
            foreach (var message in actualMessages)
            {
                expectedMessages.ShouldContain(message, 
                    $"Expected message '{message}' not found in the expected messages.\n" +
                    $"Expected messages: [{string.Join(", ", expectedMessages)}]");
            }
        }
    }
}
