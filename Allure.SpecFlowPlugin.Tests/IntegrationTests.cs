using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Allure.Commons;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Allure.SpecFlowPlugin.Tests
{
    [TestFixture]
    public class IntegrationFixture
    {
        private const string SCENARIO_PATTERN = "Scenario:";
        private const string FEATURE_PATTERN = "Feature:";
        private readonly HashSet<string> scenarioTitles = new HashSet<string>();
        private readonly HashSet<TestResultContainer> allureContainers = new HashSet<TestResultContainer>();
        private readonly HashSet<TestResult> allureTestResults = new HashSet<TestResult>();
        private readonly HashSet<TestResultContainer> specflowSuites = new HashSet<TestResultContainer>();


        [OneTimeSetUp]
        public void Init()
        {
            var testDirectory = @"..\..\..\..\Tests.SpecRun\bin\debug";
            var allureDirectory = $@"{testDirectory}\TestResults\allure-results";
            if (Directory.Exists(allureDirectory))
                Directory.Delete(allureDirectory, true);

            // run SpecFlow scenarios using SpecRun runner
            var process = Process.Start($@"{testDirectory}\\runtests.cmd");
            process.WaitForExit();

            // parse allure suites
            ParseAllureSuites(allureDirectory);
        }


        [TestCase(Status.passed, 16)]
        [TestCase(Status.failed, 1 * 2)]
        [TestCase(Status.broken, 8 * 2 + 7)]
        [TestCase(Status.skipped, 2)]
        public void TestStatus(Status status, int count)
        {
            var scenariosByStatus = allureTestResults.Where(x => x.status == status);
            Assert.That(scenariosByStatus, Has.Exactly(count).Items, scenariosByStatus.Count().ToString());
        }

        private void ParseAllureSuites(string allureResultsDir)
        {
            var allureTestResultFiles = new DirectoryInfo(allureResultsDir).GetFiles("*-result.json");
            var allureContainerFiles = new DirectoryInfo(allureResultsDir).GetFiles("*-container.json");
            var serializer = new JsonSerializer();

            foreach (var fileInfo in allureContainerFiles)
                using (var file = File.OpenText(fileInfo.FullName))
                {
                    var container = (TestResultContainer) serializer.Deserialize(file, typeof(TestResultContainer));
                    allureContainers.Add(container);
                }

            foreach (var fileInfo in allureTestResultFiles)
                using (var file = File.OpenText(fileInfo.FullName))
                {
                    var testResult = (TestResult) serializer.Deserialize(file, typeof(TestResult));
                    allureTestResults.Add(testResult);
                }
        }

        [Test]
        public void AllScenariosWithFailureTagShouldBeBroken()
        {
            var withFailureTags = allureTestResults
                .Where(x => x.labels
                    .Any(l => l.name == Label.Tag("").name && l.value.EndsWith("failed") &&
                              l.value != "afterfeaturefailed"))
                .Select(x => x.status);
            Assert.That(withFailureTags, Is.All.EqualTo(Status.broken));
        }

        [Test]
        public void ShouldConvertTableToStepParams()
        {
            var parameters = allureTestResults
                .First(x => x.name == "Table arguments").steps.SelectMany(s => s.parameters);

            Assert.That(parameters.Select(x => x.name), Has.Exactly(1).EqualTo("name"));
            Assert.That(parameters.Select(x => x.name), Has.Exactly(1).EqualTo("surname"));
            Assert.That(parameters.Select(x => x.name), Has.Exactly(2).EqualTo("width"));
            Assert.That(parameters.Select(x => x.name), Has.Exactly(0).EqualTo("attribute"));
        }

        [Test]
        public void ShouldGroupNestedSteps()
        {
            var nestedSteps = allureTestResults
                .First(x => x.name == "Shared Steps").steps
                .SelectMany(x => x.steps);

            Assert.That(nestedSteps, Has.Exactly(1).Items);
        }

        [Test]
        public void ShouldNotDuplicateAfterFixtures()
        {
            var afters = allureContainers.Select(x => x.afters.Select(y => y.name));
            Assert.That(afters, Is.All.Unique);
        }

        [Test]
        public void ShouldNotDuplicateBeforeFixtures()
        {
            var befores = allureContainers.Select(x => x.befores.Select(y => y.name));
            Assert.That(befores, Is.All.Unique);
        }

        [Test]
        public void ShouldParseLinks()
        {
            var scenarios = allureTestResults
                .Where(x => x.labels.Any(l => l.value == "labels"));

            var links = scenarios.SelectMany(x => x.links);
            Assert.Multiple(() =>
            {
                Assert.That(links.Select(x => x.url), Has.One.EqualTo("http://example.org"));
                Assert.That(links.Where(x => x.type == "tms").Select(x => x.url),
                    Has.One.EqualTo("https://example.org/234"));
                Assert.That(links.Where(x => x.type == "issue").Select(x => x.url),
                    Has.One.EqualTo("https://example.org/999999").And.One.EqualTo("https://example.org/123"));
            });
        }

        [Test]
        public void ShouldParseTags()
        {
            var scenarios = allureTestResults
                .Where(x => x.labels.Any(l => l.value == "labels"));

            var labels = scenarios.SelectMany(x => x.labels);
            Assert.Multiple(() =>
            {
                // ummatched tags
                Assert.That(labels.Where(x => x.name == "tag"), Has.Exactly(scenarios.Count() + 1).Items);
                // owner
                Assert.That(labels.Where(x => x.value == "Vasya").Select(l => l.name),
                    Has.Exactly(scenarios.Count()).Items.And.All.EqualTo("owner"));
            });
        }

        [Test]
        public void ShouldReadHostNameFromConfigTitle()
        {
            var hostNames = allureTestResults
                .SelectMany(x => x.labels.Where(l => l.name == "host").Select(l => l.value)).Distinct();
            Assert.That(hostNames, Has.One.Items.And.All.EqualTo("5994A3F7-AF84-46AD-9393-000BB45553CC"));
        }
    }
}