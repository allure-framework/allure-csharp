using System.Collections.Generic;

namespace Allure.XUnit.Examples.TestData
{
    public class TestDataGenerators
    {
        public static IEnumerable<object[]> Data =>
            new List<object[]>
            {
                new object[] {1, 2, 3},
                new object[] {-4, -6, -10},
                new object[] {-2, 2, 0},
                new object[] {int.MinValue, -1, int.MaxValue},
            };

        public static IEnumerable<object[]> TestReferenceCustomTypeGenerator = new[]
        {
            new object[] {new MyTestClass {Test = 1}, new MyTestClass {Test = 1}},
            new object[] {new MyTestClass {Test = -1}, new MyTestClass {Test = -1}},
            new object[] {new MyTestClass {Test = 10}, new MyTestClass {Test = 10}},
            new object[] {new MyTestClass {Test = 10}, new MyTestClass {Test = 11}}
        };

    }
}