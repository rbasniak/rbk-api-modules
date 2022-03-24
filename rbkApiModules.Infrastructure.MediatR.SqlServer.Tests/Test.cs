using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace rbkApiModules.Infrastructure.MediatR.SqlServer.Tests
{
    public class TestClass
    {
        [Fact]
        public void Test()
        {
            var instance = new Test1();
            instance.Add();

            foreach (var property in typeof(Test1).GetProperties())
            {
                var value = property.GetValue(instance);

                var enumerable = value as IEnumerable;

                if (enumerable != null)
                {
                    var enumerator = enumerable.GetEnumerator();
                    if (enumerator.MoveNext())
                    {
                        Debug.WriteLine("USADO");
                    }
                    else
                    {
                        Debug.WriteLine("NÃO USADO");
                    }    

                    //if (list.Count() > 0)
                    //{
                    //    isUsed = true;

                    //    break;
                    //}
                }
            }
        }

    }

    public class Test1
    {
        private HashSet<Test1> _children;

        public Test1()
        {
            _children = new HashSet<Test1>();
        }

        public void Add()
        {
            _children.Add(new Test1());
        }

        public System.Collections.Generic.IEnumerable<Test1> Children => _children?.ToList();
    }
}
