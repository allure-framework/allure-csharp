using NUnit.Framework;
using System;

namespace Allure.Commons.Tests
{
    [SetUpFixture]
    public class TestSetup
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }
    }
}
