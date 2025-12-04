namespace rbkApiModules.Commons.Core;

public static class IEnumerableExtensions
{
    public static IReadOnlyCollection<T> AsReadOnly<T>(this IEnumerable<T> source)
    {
        if (source is IReadOnlyCollection<T> readOnlyCollection)
        {
            return readOnlyCollection;
        }

        return source.ToList().AsReadOnly();
    }
}
