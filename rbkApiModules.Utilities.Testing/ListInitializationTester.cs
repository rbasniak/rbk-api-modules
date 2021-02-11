using Shouldly;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Testing
{
    public static class ListInitializationExtension
    {
        public static void ShouldHaveAllListInitialized(this object entity)
        {
            var results = VerifyListInitialization(entity);
            if (results.Length > 0)
            {
                throw new ShouldAssertException("Non initialized lists: " + String.Join(", ", results));
            }
        }

        private static string[] VerifyListInitialization(object instance)
        {
            var results = new List<string>();

            var properties = instance.GetType().GetProperties();

            foreach (var property in properties)
            {
                if (typeof(System.Collections.IEnumerable).IsAssignableFrom(property.PropertyType)
                    && property.PropertyType != typeof(string)
                    && property.PropertyType != typeof(DateTime))
                {
                    if (property.GetValue(instance) == null)
                    {
                        results.Add(property.Name);
                    }
                }
            }

            return results.ToArray();
        }
    }
}
