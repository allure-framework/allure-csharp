using Allure.Commons;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Allure.Compatibility.Tests
{
    [TestFixture]
    public class CompatibiltyTests
    {
        static AllureLifecycle cycle;

        [OneTimeSetUp]
        public void SetDir()
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(this.GetType().Assembly.Location);
            cycle = AllureLifecycle.Instance;
        }
        [Test]
        public void CycleSmokeTest()
        {
            cycle
                .StartTestCase(new TestResult() { uuid = "test" })
                .WriteTestCase("test");
        }

        [Test]
        public void ExtensionsTest()
        {
            Assert.NotNull(Label.Host());
            Assert.NotNull(Label.Thread());


        }
    }
}
