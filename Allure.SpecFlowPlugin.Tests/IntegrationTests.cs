﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Allure.Commons;
using Gherkin;
using Gherkin.Ast;
using Gherkin.Stream;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Allure.SpecFlowPlugin.Tests
{
    [TestFixture]
    public class IntegrationFixture
    {
        private readonly HashSet<TestResultContainer> allureContainers = new HashSet<TestResultContainer>();
        private readonly HashSet<TestResult> allureTestResults = new HashSet<TestResult>();
        private IEnumerable<IGrouping<string, string>> scenariosByStatus;


        [OneTimeSetUp]
        public void Init()
        {
            var featuresDirectory = @"./../../../../Allure.Features/TestData";
            var testDirectory = @"./../../../../Allure.Features/bin/Debug/netcoreapp3.1";
            var allureDirectory = $@"{testDirectory}/allure-results";
            // if (Directory.Exists(allureDirectory))
            //     Directory.Delete(allureDirectory, true);

            // run SpecFlow scenarios using SpecRun runner
            // var process = Process.Start($@"{testDirectory}\\runtests.cmd");
            // process.WaitForExit();

            // parse allure suites
            ParseAllureSuites(allureDirectory);
            ParseFeatures(featuresDirectory);
        }


        [TestCase(Status.passed)]
        [TestCase(Status.failed)]
        [TestCase(Status.broken)]
        [TestCase(Status.skipped)]
        public void TestStatus(Status status)
        {
            var expected = scenariosByStatus.FirstOrDefault(x => x.Key == status.ToString()).ToList();
            var actual = allureTestResults.Where(x => x.status == status).Select(x => x.name).ToList();
            Assert.That(actual, Is.EquivalentTo(expected));
        }


        private void ParseFeatures(string featuresDir)
        {
            var parser = new Parser();
            var scenarios = new List<Scenario>();
            var features = new DirectoryInfo(featuresDir).GetFiles("*.feature");
            scenarios.AddRange(features.SelectMany(f => parser.Parse(f.FullName).Feature.Children)
                .Select(x => x as Scenario));

            scenariosByStatus =
                scenarios.GroupBy(x => x.Tags.FirstOrDefault(x =>
                                               Enum.GetNames(typeof(Status)).Contains(x.Name.Replace("@", "")))?.Name
                                           .Replace("@", "") ??
                                       "_notag_", x => x.Name);
        }

        private void ParseAllureSuites(string allureResultsDir)
        {
            var allureTestResultFiles = new DirectoryInfo(allureResultsDir).GetFiles("*-result.json");
            var allureContainerFiles = new DirectoryInfo(allureResultsDir).GetFiles("*-container.json");
            var serializer = new JsonSerializer();

            foreach (var fileInfo in allureContainerFiles)
            {
                using var file = File.OpenText(fileInfo.FullName);
                var container = (TestResultContainer) serializer.Deserialize(file, typeof(TestResultContainer));
                allureContainers.Add(container);
            }

            foreach (var fileInfo in allureTestResultFiles)
            {
                using var file = File.OpenText(fileInfo.FullName);
                var testResult = (TestResult) serializer.Deserialize(file, typeof(TestResult));
                allureTestResults.Add(testResult);
            }
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
                // all selected scenarios should have only 2 unmatched tags - "labels" and "passed". One scenario also has "tag1" as unmatched.
                Assert.That(labels.Where(x => x.name == "tag"), Has.Exactly(scenarios.Count() * 2 + 1).Items);
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