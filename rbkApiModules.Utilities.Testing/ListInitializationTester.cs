using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Testing
{
    public class ListInitializationTester
    {
        private object _instance;

        public ListInitializationTester(object instance)
        {
            _instance = instance;
        }

        public List<string> Test()
        {
            var results = new List<string>();

            var properties = _instance.GetType().GetProperties();

            foreach (var property in properties)
            {
                if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                {
                    if (property.GetValue(_instance) == null)
                    {
                        results.Add(property.Name);
                    }
                }
            }

            return results;
        }
    }
}
