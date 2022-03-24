using Shouldly;
using System;
using System.Linq;
using rbkApiModules.Infrastructure.Models;

namespace rbkApiModules.Utilities.Testing
{
    public static class ModelValidation
    {
        public static void ShouldHaveValidationError<TException>(Action action, string expectedProperty, params ValidationType[] expectedTypes) where TException : Exception
        {
            var errors = Should.Throw<ModelValidationException>(action).Errors;

            var validationResult = errors.FirstOrDefault(x => x.PropertyName == expectedProperty);

            if (validationResult == null)
            {
                throw new ShouldAssertException($"Expected validation errors for '{expectedProperty}'");
            }

            foreach (var type in expectedTypes)
            {
                if (!validationResult.Results.Any(x => x.Type == type))
                {
                    throw new ShouldAssertException($"Expected '{type.ToString()}' validation error for '{expectedProperty}'");
                }
            }
        }
    }
}
