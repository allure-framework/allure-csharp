using System;
using System.IO;
using Allure.Net.Commons;
using NUnit.Allure.Attributes;
using NUnit.Allure.Core;
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
            StepsHelper.UpdateTestResult(tr =>
            {
                tr.name = "Some awesome name";
            });
            AllureLifecycle.Instance.AddAttachment(
                Path.Combine(TestContext.CurrentContext.TestDirectory, "allureConfig.json"),
                "AllureConfig.json");
            StepsExamples.Step1();
        }

        [TearDown]
        [AllureAfter("AllureAfter attribute description")]
        public void TearDown()
        {
            StepsExamples.Step3();
            StepsHelper.UpdateTestResult(tr =>
            {
                tr.name = "Some awesome name (changed on teardown)";
            });
            AllureLifecycle.Instance.AddAttachment(
                Path.Combine(TestContext.CurrentContext.TestDirectory, "allureConfig.json"),
                "AllureConfig.json");
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
        [AllureSubSuite("Test Subsuite")]
        public void Test()
        {
            StepsExamples.StepWithParams("first", "second");
        }
    }
}