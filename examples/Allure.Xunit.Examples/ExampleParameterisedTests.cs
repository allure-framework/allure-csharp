using System;
using System.Diagnostics;
using System.IO;
using Allure.Xunit.Attributes;
using Allure.Xunit.Examples.TestData;
using Xunit;

namespace Allure.Xunit.Examples
{
    public class ExampleParameterisedTests
    {
        public ExampleParameterisedTests()
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(GetType().Assembly.Location);
        }

        [Theory]
        [AllureParentSuite("AllTests")]
        [AllureSuite("Test AllureXunitTheory")]
        [AllureSubSuite("Test MemberData")]
        [MemberData(nameof(TestDataGenerators.Data), MemberType = typeof(TestDataGenerators))]
        public void TestTheoryWithMemberDataProperty(int value1, int value2, int expected)
        {
            var result = value1 + value2;

            Assert.Equal(expected, result);
        }

        [Theory]
        [AllureParentSuite("AllTests")]
        [AllureSuite("Test AllureXunitTheory")]
        [AllureSubSuite("Test ClassData")]
        [ClassData(typeof(TestClassData))]
        public void TestTheoryWithClassData(int value1, int value2, int expected)
        {
            var result = value1 + value2;

            Assert.Equal(expected, result);
        }

        [Theory]
        [AllureParentSuite("AllTests")]
        [AllureSuite("Test AllureXunitTheory")]
        [AllureSubSuite("Test InlineData")]
        [InlineData(1, 1)]
        [InlineData(1, 2)]
        public void TestTheory(int a, int b)
        {
            Assert.Equal(a, b);
        }

        [Theory]
        [AllureParentSuite("AllTests")]
        [AllureSuite("Test AllureXunitTheory")]
        [AllureSubSuite("Test MemberData with Custom Reference Type")]
        [MemberData(nameof(TestDataGenerators.TestReferenceCustomTypeGenerator),
            MemberType = typeof(TestDataGenerators))]
        public void TestTheoryWithMemberData(MyTestClass a, MyTestClass b)
        {
            Assert.Equal(a.Test, b.Test);
        }

        [Theory]
        [AllureParentSuite("AllTests")]
        [AllureSuite("Test AllureXunitTheory")]
        [AllureSubSuite("Test test MemberData with random data")]
        [MemberData(nameof(TestDataGenerators.RandomData), MemberType = typeof(TestDataGenerators))]
        public void TestTheoryWithMemberDataThatReturnsRandomData(int value1, int value2, int expected)
        {
            var result = value1 + value2;

            Assert.Equal(expected, result);
        }

        [Theory]
        [AllureParentSuite("AllTests")]
        [AllureSuite("Test AllureXunitTheory")]
        [AllureSubSuite("Test test with generic arguments")]
        [InlineData(5, 10)]
        [InlineData(10, 20)]
        public void TestTheoryWithInlineDataThatAcceptsGenericArgument(int? value, int? expected)
        {
            var result = value * 2;

            Debug.Assert(expected != null, nameof(expected) + " != null");
            Debug.Assert(result != null, nameof(result) + " != null");
            Assert.Equal(expected.Value, result.Value);
        }
    }
}