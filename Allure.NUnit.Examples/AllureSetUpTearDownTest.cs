using System;
using NUnit.Allure.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Examples
{
    [AllureSuite("Tests - SetUp/TearDown")]
    public class AllureSetUpTearDownTest : BaseTest
    {
        [SetUp]
        [AllureBefore("AllureBefore attribute description")]
        public void SetUp()
        {
            Console.WriteLine("I'm an unwrapped SetUp");
            StepsExamples.Step1();
        }

        [TearDown]
        [AllureAfter("AllureAfter attribute description")]
        public void TearDown()
        {
            StepsExamples.Step3();
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Console.WriteLine("I'm an unwrapped OneTimeSetUp");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Console.WriteLine("I'm an unwrapped OneTimeTearDown");
        }

        [Test]
        [AllureSubSuite("Test")]
        public void Test()
        {
            StepsExamples.StepWithParams("first", "second");
        }
    }
}