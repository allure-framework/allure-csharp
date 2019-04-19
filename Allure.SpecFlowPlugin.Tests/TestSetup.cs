using System;
using System.IO;
using NUnit.Framework;

namespace Allure.SpecFlowPlugin.Tests
{
    [SetUpFixture]
    public class TestSetup
    {
        [OneTimeSetUp]
        public void Setup()
        {
            // setup current folder for nUnit engine
            Environment.CurrentDirectory = Path.GetDirectoryName(typeof(TestSetup).Assembly.Location);
        }
    }
}