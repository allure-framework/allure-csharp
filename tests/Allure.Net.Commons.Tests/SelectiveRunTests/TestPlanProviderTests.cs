using Allure.Net.Commons.TestPlan;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;

namespace Allure.Net.Commons.Tests.SelectiveRunTests
{
    class TestPlanProviderTests
    {
        private string testPlanPath;

        [SetUp]
        public void SetUp()
        {
            this.testPlanPath = Path.GetTempFileName();
            File.WriteAllText(
                this.testPlanPath,
                "{\"tests\": [{\"id\": \"100\"}]}",
                Encoding.UTF8
            );
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(this.testPlanPath))
            {
                File.Delete(this.testPlanPath);
            }
            Environment.SetEnvironmentVariable("ALLURE_TESTPLAN_PATH", null);
            Environment.SetEnvironmentVariable("AS_TESTPLAN_PATH", null);
        }

        [Test]
        public void TestPlanCreatedFromFileByNewEnvName()
        {
            Environment.SetEnvironmentVariable(
                "ALLURE_TESTPLAN_PATH",
                this.testPlanPath
            );

            var testplan = AllureTestPlan.FromEnvironment();

            Assert.That(
                testplan.Tests,
                Is.EqualTo(
                    new[] { new AllureTestPlanItem() { Id = "100" } }
                )
            );
        }

        [Test]
        public void TestPlanCreatedFromFileByOldEnvName()
        {
            Environment.SetEnvironmentVariable(
                "AS_TESTPLAN_PATH",
                this.testPlanPath
            );

            var testplan = AllureTestPlan.FromEnvironment();

            Assert.That(
                testplan.Tests,
                Is.EqualTo(
                    new[] { new AllureTestPlanItem() { Id = "100" } }
                )
            );
        }

        [Test]
        public void DefaultTestPlanCreatedIfNoEnvVarDefined()
        {
            Assert.That(
                AllureTestPlan.FromEnvironment(),
                Is.SameAs(
                    AllureTestPlan.DEFAULT_TESTPLAN
                )
            );
        }

        [Test]
        public void DefaultTestPlanCreatedIfFIleDoesntExist()
        {
            Environment.SetEnvironmentVariable(
                "ALLURE_TESTPLAN_PATH",
                Guid.NewGuid().ToString()
            );

            Assert.That(
                AllureTestPlan.FromEnvironment(),
                Is.SameAs(
                    AllureTestPlan.DEFAULT_TESTPLAN
                )
            );
        }

        [Test]
        public void IfBothEnvVarsPresentNewIsPreferred()
        {
            Environment.SetEnvironmentVariable(
                "AS_TESTPLAN_PATH",
                Guid.NewGuid().ToString()
            );
            Environment.SetEnvironmentVariable(
                "ALLURE_TESTPLAN_PATH",
                this.testPlanPath
            );

            var testplan = AllureTestPlan.FromEnvironment();

            Assert.That(
                testplan.Tests,
                Is.EqualTo(
                    new[] { new AllureTestPlanItem() { Id = "100" } }
                )
            );
        }
    }
}
