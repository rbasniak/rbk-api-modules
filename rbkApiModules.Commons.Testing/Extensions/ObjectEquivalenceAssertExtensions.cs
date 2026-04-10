using System.Collections.ObjectModel;
using Shouldly;
using System.Linq.Expressions;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;
using rbkApiModules.Testing.Core;
using rbkApiModules.Commons.Core.Abstractions;

namespace rbkApiModules.Commons.Testing;

public static class AssertExtensions
{

    public static void ShouldBeEquivalentToCollection<T>(this HttpResponse<T[]> response, object expected) where T : class
    {
        response.ShouldHaveEquivalentToCollection(expected, null, "Collections are not equivalent");
    }

    public static void ShouldBeEquivalentToCollection<T>(this HttpResponse<T[]> response, object expected, string customMessage) where T : class
    {
        response.ShouldHaveEquivalentToCollection(expected, null, customMessage);
    }

    public static void ShouldBeEquivalentToCollection<T>(this HttpResponse<T[]> response, object expected, Action<EntityAssertionOptions<T>>? configuration) where T : class
    {
        response.ShouldBeSuccess(out T[] result);

        result.ShouldBeEquivalentToCollection(expected, configuration, "Collections are not equivalent");
    }

    public static void ShouldHaveEquivalentToCollection<T>(this HttpResponse<T[]> response, object expected, Action<EntityAssertionOptions<T>>? configuration, string customMessage) where T : class
    {
        response.ShouldBeSuccess(out T[] result);

        result.ShouldBeEquivalentToCollection(expected, configuration, customMessage);
    }

    public static void ShouldHaveEquivalentObject<T>(this HttpResponse<T> response, object expected) where T : class
    {
        response.ShouldHaveEquivalentObject(expected, null, "Objects are not equivalent");
    }

    public static void ShouldHaveEquivalentObject<T>(this HttpResponse<T> response, object expected, string customMessage) where T : class
    {
        response.ShouldHaveEquivalentObject(expected, null, customMessage);
    }

    public static void ShouldHaveEquivalentObject<T>(this HttpResponse<T> response, object expected, Action<EntityAssertionOptions<T>>? configuration) where T : class
    {
        response.ShouldBeSuccess(out T result);

        result.ShouldBeEquivalentToObject(expected, configuration, "Objects are not equivalent");
    }

    public static void ShouldHaveEquivalentObject<T>(this HttpResponse<T> response, object expected, Action<EntityAssertionOptions<T>>? configuration, string customMessage) where T : class
    {
        response.ShouldBeSuccess(out T result);

        result.ShouldBeEquivalentToObject(expected, configuration, customMessage);
    }

    public static void ShouldBeEquivalentToObject<T>(this T actual, object expected)
    {
        actual.ShouldBeEquivalentToObject(expected, null, "Objects are not equivalent");
    }

    public static void ShouldBeEquivalentToObject<T>(this T actual, object expected, string customMessage)
    {
        actual.ShouldBeEquivalentToObject(expected, null, customMessage);
    }

    public static void ShouldBeEquivalentToObject<T>(this T actual, object expected, Action<EntityAssertionOptions<T>>? configuration)
    {
        actual.ShouldBeEquivalentToObject(expected, configuration, "Objects are not equivalent");
    }

    public static void ShouldBeEquivalentToObject<T>(this T actual, object expected, Action<EntityAssertionOptions<T>>? configuration, string customMessage)
    {
        CompareObjects(actual, expected, configuration, customMessage);
    }

    public static void ShouldBeEquivalentToCollection<T>(this IEnumerable<T> actual, object expected, Action<EntityAssertionOptions<T>>? configuration, string customMessage)
    {
        CompareLists(actual, expected, configuration, customMessage);
    }

    private static void CompareObjects<T>(object actual, object expected, Action<EntityAssertionOptions<T>>? configuration, string customMessage)
    {
        var options = new EntityAssertionOptions<T>();

        if (configuration != null)
        {
            configuration(options);
        }

        actual.ShouldNotBeNull($"The actual value passed to {nameof(ShouldBeEquivalentToObject)} should not be null");

        foreach (var propertyOnActualObject in actual.GetType().GetProperties())
        {
            if (propertyOnActualObject.Name == "SharedNetworkFolder")
            {
                // Debugger.Break();
            }

            if (options.IgnoredProperties.Contains(propertyOnActualObject.Name)) continue;

            var propertyFromExpectedObject = expected.GetType().GetProperty(propertyOnActualObject.Name);

            propertyFromExpectedObject.ShouldNotBeNull($"The property {propertyOnActualObject.Name} on actual object {actual.GetType().Name} was not found on expected object {expected.GetType().Name}");

            var valueFromExpectedObject = propertyFromExpectedObject.GetValue(expected);
            var valueFromActualObject = propertyOnActualObject.GetValue(actual);

            if (valueFromExpectedObject != null && valueFromActualObject != null)
            {
                var friendlyPropertyName = $"{actual.GetType().Name}.{propertyOnActualObject.Name}";

                if (valueFromActualObject is Guid && valueFromExpectedObject is string)
                {
                    valueFromActualObject.ToString().ToLower().ShouldBe(valueFromExpectedObject.ToString().ToLower());
                }
                else if (valueFromActualObject is string && valueFromExpectedObject is Guid)
                {
                    valueFromActualObject.ToString().ToLower().ShouldBe(valueFromExpectedObject.ToString().ToLower());
                }
                else if (valueFromActualObject is string && valueFromExpectedObject is string) // Needed because strings are IEnumerable, to avoid they falling in the IEnumerables condition
                {
                    valueFromActualObject.ToString().ShouldBe(valueFromExpectedObject.ToString(), $"{friendlyPropertyName}: strings didn't match");
                }
                else if (valueFromActualObject is System.Collections.IEnumerable && valueFromExpectedObject is System.Collections.IEnumerable)
                {
                    CompareLists(valueFromActualObject, valueFromExpectedObject, configuration, customMessage);
                }
                else if (valueFromActualObject is EnumReference && valueFromExpectedObject is Enum)
                {
                    var castValueFromActualObject = (EnumReference)valueFromActualObject;
                    var castValueFromExpectedObject = (Enum)valueFromExpectedObject;

                    var enumType = valueFromExpectedObject.GetType();
                    var memberInfos = enumType.GetMember(valueFromExpectedObject.ToString());
                    var enumValueMemberInfo = memberInfos.FirstOrDefault(x => x.DeclaringType == enumType);
                    var valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

                    string description;

                    if (valueAttributes.Length > 0)
                    {
                        description = ((DescriptionAttribute)valueAttributes[0]).Description;
                    }
                    else
                    {
                        description = valueFromExpectedObject.ToString();
                    }

                    castValueFromActualObject.Id.ShouldBe((int)valueFromExpectedObject, $"{friendlyPropertyName}: when comparing Enum to SimpleNamedEntity<int>, the Id didn't match");
                    castValueFromActualObject.Value.ShouldBe(description, $"{friendlyPropertyName}: when comparing Enum to SimpleNamedEntity<int>, the Name didn't match");
                }
                else if (valueFromActualObject is EnumReference && valueFromExpectedObject is Enum)
                {
                    var castValueFromActualObject = (EnumReference)valueFromActualObject;
                    var castValueFromExpectedObject = (Enum)valueFromExpectedObject;

                    var enumType = valueFromExpectedObject.GetType();
                    var memberInfos = enumType.GetMember(valueFromExpectedObject.ToString());
                    var enumValueMemberInfo = memberInfos.FirstOrDefault(x => x.DeclaringType == enumType);
                    var valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

                    string description;

                    if (valueAttributes.Length > 0)
                    {
                        description = ((DescriptionAttribute)valueAttributes[0]).Description;
                    }
                    else
                    {
                        description = valueFromExpectedObject.ToString();
                    }

                    castValueFromActualObject.Id.ShouldBe((int)valueFromExpectedObject, $"{friendlyPropertyName}: when comparing Enum to SimpleNamedEntity<int>, the Id didn't match");
                    castValueFromActualObject.Value.ShouldBe(description, $"{friendlyPropertyName}: when comparing Enum to SimpleNamedEntity<int>, the Name didn't match");
                }
                else if (valueFromActualObject is EntityReference)
                {
                    // Ver se o expected tem property.Id e property.Nome, se nao tiver, ver se tem propertyId
                }
                else if (valueFromExpectedObject is EntityReference)
                {

                }
                else
                {
                    valueFromActualObject.GetType().ShouldBe(valueFromExpectedObject.GetType(), $"{friendlyPropertyName}: types on actual and expected objects didn't match");

                    if (valueFromActualObject is string && valueFromExpectedObject is string && options.PropertiesToIgnoreCase.Contains(propertyOnActualObject.Name))
                    {
                        var valueFromActualObjectStr = valueFromActualObject.ToString();
                        var valueFromExpectedObjectStr = valueFromExpectedObject.ToString();

                        valueFromActualObjectStr.ShouldBe(valueFromExpectedObjectStr, $"Value on actual and expected objects for property {propertyOnActualObject.Name} didn't match");
                    }
                    else
                    {
                        valueFromActualObject.ShouldBe(valueFromExpectedObject, $"Value on actual and expected objects for property {propertyOnActualObject.Name} didn't match");
                    }
                }
            }
        }
    }

    private static void CompareLists<T>(object actual, object expected, Action<EntityAssertionOptions<T>>? configuration, string customMessage)
    {
        var expectedList = new List<object>();
        var actualList = new List<object>();

        if (actual is System.Collections.IEnumerable actualEnumerable)
        {
            var getActualEnumeratorMethod = actualEnumerable.GetType().GetMethod("GetEnumerator");
            var actualEnumerator = (System.Collections.IEnumerator)getActualEnumeratorMethod.Invoke(actualEnumerable, null);

            while (actualEnumerator.MoveNext())
            {
                actualList.Add(actualEnumerator.Current);
            }
        }
        else
        {
            throw new NotSupportedException();
        }

        if (expected is System.Collections.IEnumerable expectedEnumerable)
        {
            var getExpectedEnumeratorMethod = expectedEnumerable.GetType().GetMethod("GetEnumerator");
            var expectedEnumerator = (System.Collections.IEnumerator)getExpectedEnumeratorMethod.Invoke(expectedEnumerable, null);

            while (expectedEnumerator.MoveNext())
            {
                expectedList.Add(expectedEnumerator.Current);
            }
        }
        else
        {
            throw new InvalidOperationException("Expected value is not of IEnumerable");
        }

        actualList.Count().ShouldBe(expectedList.Count(), "The number of the values in the lists doesn't match");

        for (int i = 0; i < actualList.Count(); i++)
        {
            CompareObjects<T>(actualList[i], expectedList[i], configuration, customMessage);
        }
    }
}

public enum PropertyComparisonMode
{
    FromSourceToDestination,
    FromDestinationToSource,
}

public class EntityAssertionOptions<T>
{
    private readonly List<string> _ignoredProperties = new List<string>();
    private readonly List<string> _propertiesToIgnoreCase = new List<string>();
    private readonly List<Action<T>> _assertActions = new List<Action<T>>();

    internal ReadOnlyCollection<string> IgnoredProperties => new ReadOnlyCollection<string>(_ignoredProperties);
    internal ReadOnlyCollection<string> PropertiesToIgnoreCase => new ReadOnlyCollection<string>(_ignoredProperties);
    internal ReadOnlyCollection<Action<T>> AssertActions => new ReadOnlyCollection<Action<T>>(_assertActions);

    public EntityAssertionOptions<T> IgnoreProperty<TKey>(Expression<Func<T, TKey>> selector)
    {
        _ignoredProperties.Add(((MemberExpression)selector.Body).Member.Name);

        return this;
    }

    public EntityAssertionOptions<T> IgnoreCase<TKey>(Expression<Func<T, TKey>> selector)
    {
        _propertiesToIgnoreCase.Add(((MemberExpression)selector.Body).Member.Name);

        return this;
    }

    public void AddAdditionaAssert(Action<T> assertAction)
    {
        _assertActions.Add(assertAction);
    }
}

