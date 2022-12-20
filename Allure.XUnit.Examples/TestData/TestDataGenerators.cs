using System;
using System.Collections.Generic;
using System.Linq;

namespace Allure.XUnit.Examples.TestData
{
    public class TestDataGenerators
    {
        private static readonly object SyncLock = new();
        private static readonly Random Random = new();
        
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

        public static IEnumerable<object[]> RandomData =>
            Enumerable.Range(0, 4).Select(_ => CreateTestData());

        private static object[] CreateTestData()
        {
            return new object[] {GetRandomNumber(), GetRandomNumber(), GetRandomNumber()};
        }

        private static int GetRandomNumber()
        {
            lock(SyncLock) 
            {
                return Random.Next();
            }
        }
    }
}