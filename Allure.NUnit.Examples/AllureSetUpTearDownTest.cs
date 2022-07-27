using System;
using System.Threading;
using NUnit.Allure.Attributes;
using NUnit.Allure.Core;
using NUnit.Framework;

namespace Allure.NUnit.Examples
{
    [AllureSuite("Tests - SetUp/TearDown")]
    public class AllureSetUpTearDownTest : BaseTest
    {
        [SetUp]
        public void SetUp()
        {
            Console.WriteLine("I'm an unwrapped SetUp");
        }

        [TearDown]
        public void TearDown()
        {
            AllureExtensions.WrapSetUpTearDownParams(() =>
            {
                Thread.Sleep(750);
                Console.WriteLine("Example of wrapped TearDown");
            }, "Custom TearDown name here");
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
        }
    }
}