using System;
using System.Collections.Generic;
using System.Reflection;
using Allure.Net.Commons;
using Allure.Net.Commons.Steps;
using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Examples
{
    [AllureSuite("Tests - Step Names")]
    public class AllureStepNameInternalsTest : BaseTest
    {
        internal class LocalTestClass
        {
            public string TestField;
            public string TestProp { get; set; }

            public void TestMethod(string name, LocalTestClass request, int id)
            {
                Console.WriteLine($"{id} - {name} ({request})");
            }
        }

        [TestCase("", ExpectedResult = "")]
        [TestCase(null, ExpectedResult = "")]
        [TestCase("{0} - {1} - {2} - {3} - {100} - {-100}", ExpectedResult = "\"Super Mario\" - {\"TestField\":\"FieldValue\",\"TestProp\":\"PropValue\"} - 12345 - {3} - {100} - {-100}")]
        [TestCase("{id} - {0}", ExpectedResult = "12345 - \"Super Mario\"")]
        [TestCase("{id} - {name} ({request})", ExpectedResult = "12345 - \"Super Mario\" ({\"TestField\":\"FieldValue\",\"TestProp\":\"PropValue\"})")]
        [TestCase("{id} - {request.TestField} - {request.TestProp}", ExpectedResult = "12345 - \"FieldValue\" - \"PropValue\"")]
        [TestCase("{notExistingParameter} - {request.NotExistingField}", ExpectedResult = "{notExistingParameter} - {request.NotExistingField}")]
        public string ApplyValues_Test(string stepNamePattern)
        {
            MethodBase methodBase = typeof(LocalTestClass).GetMethod(nameof(LocalTestClass.TestMethod))!;
            object[] arguments = new object[]
            {
                "Super Mario", // name = {0}
                new LocalTestClass // request = {1}
                {
                    TestField = "FieldValue",
                    TestProp = "PropValue",
                },
                12345, // id = {2}
            };

            return AllureStepParameterHelper.GetStepName(
                stepNamePattern,
                methodBase,
                arguments,
                new Dictionary<Type, ITypeFormatter>()
            );
        }

        [TestCase("", ExpectedResult = "")]
        [TestCase(null, ExpectedResult = "")]
        [TestCase("{0} - {1} - {2} - {3} - {100} - {-100}", ExpectedResult = "\"Super Mario\" - null - 12345 - {3} - {100} - {-100}")]
        [TestCase("{id} - {0}", ExpectedResult = "12345 - \"Super Mario\"")]
        [TestCase("{id} - {name} ({request})", ExpectedResult = "12345 - \"Super Mario\" (null)")]
        [TestCase("{id} - {request.TestField} - {request.TestProp}", ExpectedResult = "12345 - {request.TestField} - {request.TestProp}")]
        [TestCase("{notExistingParameter} - {request.NotExistingField}", ExpectedResult = "{notExistingParameter} - {request.NotExistingField}")]
        public string ApplyNullValues_Test(string stepNamePattern)
        {
            MethodBase methodBase = typeof(LocalTestClass).GetMethod(nameof(LocalTestClass.TestMethod))!;
            object[] arguments = new object[]
            {
                "Super Mario", // name = {0}
                null,
                12345, // id = {2}
            };

            return AllureStepParameterHelper.GetStepName(
                stepNamePattern,
                methodBase,
                arguments,
                new Dictionary<Type, ITypeFormatter>()
            );
        }
    }
}
