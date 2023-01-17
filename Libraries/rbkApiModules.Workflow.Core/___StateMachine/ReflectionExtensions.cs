using System.Reflection;

namespace Stateless
{
    internal static class ReflectionExtensions
    {
        /// <summary>
        ///     Convenience method to get <see cref="MethodInfo" /> for different PCL profiles.
        /// </summary>
        /// <param name="del">Delegate whose method info is desired</param>
        /// <returns>Null if <paramref name="del" /> is null, otherwise <see cref="MemberInfo.Name" />.</returns>
        public static MethodInfo TryGetMethodInfo(this Delegate del)
        {
            return del?.Method;
        }

        /// <summary>
        ///     Convenience method to get method name for different PCL profiles.
        /// </summary>
        /// <param name="del">Delegate whose method name is desired</param>
        /// <returns>Null if <paramref name="del" /> is null, otherwise <see cref="MemberInfo.Name" />.</returns>
        public static string TryGetMethodName(this Delegate del)
        {
            return TryGetMethodInfo(del)?.Name;
        }
    }
}