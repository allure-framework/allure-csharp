using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
