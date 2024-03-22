using NUnit.Framework;
using System;
using System.IO;

namespace Allure.SpecFlowPlugin.Tests
{
    [SetUpFixture]
    public class TestSetup
    {
        string? originalCwd = null;

        [OneTimeSetUp]
        public void Setup()
        {
            // setup current folder for nUnit engine
            var directory = Path.GetDirectoryName(
                typeof(TestSetup).Assembly.Location
            );
            if (directory is not null)
            {
                this.originalCwd = Environment.CurrentDirectory;
                Environment.CurrentDirectory = directory;
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            if (this.originalCwd is not null)
            {
                Environment.CurrentDirectory = originalCwd;
            }
        }
    }
}