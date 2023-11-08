using System;
using System.IO;
using Allure.Net.Commons;
using NUnit.Allure;
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
            AllureLifecycle.Instance.UpdateTestCase(tr =>
            {
                tr.name = "Some awesome name";
            });
            Attachments.File("AllureConfig.json", Path.Combine(TestContext.CurrentContext.TestDirectory, "allureConfig.json"));
            StepsExamples.Step1();
        }

        [TearDown]
        [AllureAfter("AllureAfter attribute description")]
        public void TearDown()
        {
            StepsExamples.Step3();
            AllureLifecycle.Instance.UpdateTestCase(tr =>
            {
                tr.name = "Some awesome name (changed on teardown)";
            });
            Attachments.File("AllureConfig.json", Path.Combine(TestContext.CurrentContext.TestDirectory, "allureConfig.json"));
        }

        [OneTimeSetUp]
        [AllureBefore("OneTimeSetUp AllureBefore attribute description")]
        public void OneTimeSetUp()
        {
            Console.WriteLine("I'm an unwrapped OneTimeSetUp");
        }

        [OneTimeTearDown]
        [AllureAfter("OneTimeTearDown AllureAfter attribute description")]
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