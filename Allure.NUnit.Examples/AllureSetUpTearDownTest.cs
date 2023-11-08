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
            AllureApi.SetName("Fixture name changed in SetUp");
            Attachments.File("AllureConfig.json", Path.Combine(TestContext.CurrentContext.TestDirectory, "allureConfig.json"));
            StepsExamples.Step1();
        }

        [TearDown]
        [AllureAfter("AllureAfter attribute description")]
        public void TearDown()
        {
            StepsExamples.Step3();
            AllureApi.SetName("Fixture name changed in TearDown");
            Attachments.File("AllureConfig.json", Path.Combine(TestContext.CurrentContext.TestDirectory, "allureConfig.json"));
        }

        [OneTimeSetUp]
        [AllureBefore("OneTimeSetUp AllureBefore attribute description")]
        public void OneTimeSetUp()
        {
            AllureApi.SetName("Fixture name changed in OneTimeSetUp");
            Console.WriteLine("I'm an unwrapped OneTimeSetUp");
        }

        [OneTimeTearDown]
        [AllureAfter("OneTimeTearDown AllureAfter attribute description")]
        public void OneTimeTearDown()
        {
            AllureApi.SetName("Fixture name changed in OneTimeTearDown");
            Console.WriteLine("I'm an unwrapped OneTimeTearDown");
        }

        [Test]
        [AllureSubSuite("Test Subsuite")]
        public void Test()
        {
            AllureApi.SetName("Test name changed in the test's body");
            StepsExamples.StepWithParams("first", "second");
        }
    }
}