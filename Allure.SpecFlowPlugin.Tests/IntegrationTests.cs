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

            var configuration = new DirectoryInfo(dir).Name;
            var scenariosProject = "Tests.SpecRun";

            var allureDirectory = $@"..\..\..\{scenariosProject}\bin\TestResults\allure-results";
            if (Directory.Exists(allureDirectory))
                Directory.Delete(allureDirectory, true);

            // run SpecFlow scenarios using SpecRun runner
            var process = Process.Start($@"..\..\..\{scenariosProject}\bin\{configuration}\runtests.cmd");
            process.WaitForExit();

            // parse allure suites
            ParseAllureSuites(allureDirectory);
        }


        [TestCase(Status.passed, 8)]
        [TestCase(Status.failed, 2)]
        [TestCase(Status.broken, 18)]
        public void TestStatus(Status status, int count)
        {
            var scenariosByStatus = allureTestResults.Where(x => x.status == status);
            Assert.That(scenariosByStatus, Has.Exactly(count).Items);
        }

        [Test]
        public void ShouldNotDuplicateBeforeFixtures()
        {
            var befores = allureContainers.Select(x => x.befores.Select(y => y.name));
            Assert.That(befores, Is.All.Unique);
        }

        [Test]
        public void ShouldNotDuplicateAfterFixtures()
        {
            var afters = allureContainers.Select(x => x.afters.Select(y => y.name));
            Assert.That(afters, Is.All.Unique);
        }

        [Test]
        public void AllScenariosWithFailureTagShouldBeBroken()
        {
            var withFailureTags = allureTestResults
                .Where(x => x.labels
                .Any(l => l.name == Label.Tag("").name && l.value.EndsWith("failed") && l.value != "afterfeaturefailed"))
                .Select(x => x.status);
            Assert.That(withFailureTags, Is.All.EqualTo(Status.broken));
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
