using System;
using System.IO;
using Allure.Xunit.Attributes;
using Allure.XUnit.Examples.TestData;
using Xunit;

namespace Allure.XUnit.Examples
{
    public class ExampleParameterisedTests
    {
      
        public ExampleParameterisedTests()
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(GetType().Assembly.Location);
        }

        [AllureXunitTheory]
        [AllureParentSuite("AllTests")]
        [AllureSuite("Test AllureXunitTheory")]
        [AllureSubSuite("Test MemberData")]
        [MemberData(nameof(TestDataGenerators.Data), MemberType = typeof(TestDataGenerators))]
        public void TestTheoryWithMemberDataProperty(int value1, int value2, int expected)
        {
            var result = value1 + value2;

            Assert.Equal(expected, result);
        }

        [AllureXunitTheory]
        [AllureParentSuite("AllTests")]
        [AllureSuite("Test AllureXunitTheory")]
        [AllureSubSuite("Test ClassData")]
        [ClassData(typeof(TestClassData))]
        public void TestTheoryWithClassData(int value1, int value2, int expected)
        {
            var result = value1 + value2;

            Assert.Equal(expected, result);
        }

        [AllureXunitTheory]
        [AllureParentSuite("AllTests")]
        [AllureSuite("Test AllureXunitTheory")]
        [AllureSubSuite("Test InlineData")]
        [InlineData(1, 1)]
        [InlineData(1, 2)]
        public void TestTheory(int a, int b)
        {
            Assert.Equal(a, b);
        }
        
        [AllureXunitTheory]
        [AllureParentSuite("AllTests")]
        [AllureSuite("Test AllureXunitTheory")]
        [AllureSubSuite("Test MemberData with Custom Reference Type")]
        [MemberData(nameof(TestDataGenerators.TestReferenceCustomTypeGenerator),
            MemberType = typeof(TestDataGenerators))]
        public void TestTheoryWithMemberData(MyTestClass a, MyTestClass b)
        {
            Assert.Equal(a.Test, b.Test);
        }
    }
}
