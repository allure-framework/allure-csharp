using System;
using System.Collections;
using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Examples
{
    [AllureDisplayIgnored]
    [AllureSuite("Tests - Ignored")]
    public class AllureIgnoredTest : BaseTest
    {
        private static IEnumerable Data
        {
            get
            {
                yield return new TestCaseData("Ignore").SetName("{m}_NotExist");
                yield return new TestCaseData("NotIgnore").SetName("{m}_NotExist ignored").Ignore("Test");
            }
        }

        [Test]
        [Ignore("Ignored test")]
        public void IgnoredTest()
        {
        }


        [TestCase("a")]
        [TestCase("b")]
        [Ignore("Foo")]
        public void IgnoredTestCase(string data)
        {
        }


        [Test]
        [TestCase("a")]
        [TestCase("b", Ignore = "Case")]
        public void IgnoreTestWithTestCaseParams(string data)
        {
            Console.WriteLine(data);
        }


        [Test]
        [TestCaseSource(typeof(AllureIgnoredTest), nameof(Data))]
        public void IgnoreTestWithTestCaseData(string data)
        {
            Console.WriteLine(data);
        }
    }
}