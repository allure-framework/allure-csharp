using Allure.Commons;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allure.SpecFlowPlugin.Tests
{
    [TestFixture]
    public class IntegrationFixture
    {
        const string SCENARIO_PATTERN = "Scenario:";
        const string FEATURE_PATTERN = "Feature:";

        HashSet<string> scenarioTitles = new HashSet<string>();
        HashSet<TestResultContainer> allureContainers = new HashSet<TestResultContainer>();
        HashSet<TestResult> allureTestResults = new HashSet<TestResult>();

        HashSet<TestResultContainer> specflowSuites = new HashSet<TestResultContainer>();


        [OneTimeSetUp]
        public void Init()
        {
            // setup current folder for nUnit engine
            var dir = Path.GetDirectoryName(typeof(IntegrationFixture).Assembly.Location);
            Environment.CurrentDirectory = dir;

            //ParseSpecFlowFeatures();

            // run SpecFlow scenarios using SpecRun runner
            var process = Process.Start(@"..\..\..\Allure.SpecFlowPlugin.Tests.Data\runtests.cmd");
            process.WaitForExit();

            // parse allure suites
            ParseAllureSuites(@"..\..\..\Allure.SpecFlowPlugin.Tests.Data\bin\allure-results");
        }


        [TestCase(Status.passed, 10)]
        [TestCase(Status.failed, 1)]
        [TestCase(Status.broken, 8)]
        public void TestStatus(Status status, int count)
        {
            Assert.AreEqual(count, allureTestResults.Where(x => x.status == status).Count());
        }

        [Test]
        public void ShouldNotDuplicateBeforeFixtures()
        {
            foreach (var befores in allureContainers.Select(x => x.befores.Select(y => y.name)))
            {
                Assert.That(befores, Is.All.Unique);
            }
        }

        [Test]
        public void ShouldNotDuplicateAfterFixtures()
        {
            foreach (var afters in allureContainers.Select(x => x.afters.Select(y => y.name)))
            {
                Assert.That(afters, Is.All.Unique);
            }
        }

        private void ParseAllureSuites(string allureResultsDir)
        {
            var allureTestResultFiles = new DirectoryInfo(allureResultsDir).GetFiles("*-result.json");
            var allureContainerFiles = new DirectoryInfo(allureResultsDir).GetFiles("*-container.json");
            JsonSerializer serializer = new JsonSerializer();

            foreach (var fileInfo in allureContainerFiles)
            {
                using (StreamReader file = File.OpenText(fileInfo.FullName))
                {
                    var container = (TestResultContainer)serializer.Deserialize(file, typeof(TestResultContainer));
                    allureContainers.Add(container);
                }
            }

            foreach (var fileInfo in allureTestResultFiles)
            {
                using (StreamReader file = File.OpenText(fileInfo.FullName))
                {
                    var testResult = (TestResult)serializer.Deserialize(file, typeof(TestResult));
                    allureTestResults.Add(testResult);
                }
            }

        }
    }
}
