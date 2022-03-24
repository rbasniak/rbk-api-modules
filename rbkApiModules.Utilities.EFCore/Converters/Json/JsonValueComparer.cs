using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using System;

namespace rbkApiModules.Utilities.EFCore
{
    public class JsonValueConverter<T> : ValueConverter<T, string> where T : class
    {
        public JsonValueConverter(ConverterMappingHints hints = default) :
          base(v => JsonHelper.Serialize(v), v => JsonHelper.Deserialize<T>(v), hints)
        { }
    }

    internal class JsonValueComparer<T> : ValueComparer<T>
    {

        private static string Json(T instance)
        {
            return JsonConvert.SerializeObject(instance);
        }

        private static T DoGetSnapshot(T instance)
        {

            if (instance is ICloneable cloneable)
                return (T)cloneable.Clone();

            var result = (T)JsonConvert.DeserializeObject(Json(instance), typeof(T));
            return result;
        }

        private static int DoGetHashCode(T instance)
        {
            if (instance is IEquatable<T>)
                return instance.GetHashCode();

            return Json(instance).GetHashCode();
        }

        private static bool DoEquals(T left, T right)
        {
            if (left is IEquatable<T> equatable)
                return equatable.Equals(right);

            var result = Json(left).Equals(Json(right));
            return result;
        }

        public JsonValueComparer() : base(
          (t1, t2) => DoEquals(t1, t2),
          t => DoGetHashCode(t),
          t => DoGetSnapshot(t))
        {
        }

    }
}
