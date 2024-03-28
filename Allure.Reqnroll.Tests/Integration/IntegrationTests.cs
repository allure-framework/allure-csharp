using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Allure.Net.Commons;
using Gherkin;
using Gherkin.Ast;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Allure.ReqnrollPlugin.Tests.Integration;

class IntegrationTests
{
    private FileInfo? allureConfigFile;
    private DirectoryInfo? allureResultsDir;
    static List<TestResultContainer>? containers;
    static List<TestResult>? results;
    static Dictionary<string, List<string>>? scenariosByStatus;

    [OneTimeSetUp]
    public void Init()
    {
        var samplesProjectDir = Path.GetFullPath(
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                @"./../../../../Allure.Reqnroll.Tests.Samples"
            )
        );
        this.allureResultsDir = Directory.CreateDirectory(
            Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid().ToString()
            )
        );
        this.allureConfigFile = PrepareAllureConfig(
            Path.Combine(
                samplesProjectDir,
                "allureConfig.json"
            ),
            this.allureResultsDir
        );

        RunSamples(samplesProjectDir, this.allureConfigFile);

        var featuresDirectory = Path.Combine(samplesProjectDir, "Features");


        containers = ParseResultFiles<TestResultContainer>(
            this.allureResultsDir.FullName,
            "*-container.json"
        );
        results = ParseResultFiles<TestResult>(
            this.allureResultsDir.FullName,
            "*-result.json"
        );
        scenariosByStatus = ParseFeatures(featuresDirectory);
    }

    [OneTimeTearDown]
    public void RemoveTempFiles()
    {
        if (this.allureResultsDir?.Exists is true)
        {
            this.allureResultsDir.Delete(recursive: true);
        }
        if (this.allureConfigFile?.Exists is true)
        {
            this.allureConfigFile.Delete();
        }
    }

    static FileInfo PrepareAllureConfig(string originalConfigPath, DirectoryInfo resultsDir)
    {
        var json = JObject.Parse(
            File.ReadAllText(originalConfigPath)
        );

        if (json["allure"] is not JObject allureJsonConfig)
        {
            throw new InvalidOperationException($"Unexpected format of {originalConfigPath}");
        }

        var path = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid().ToString()
        );
        allureJsonConfig["directory"] = new JValue(resultsDir.FullName);
        File.WriteAllText(path, json.ToString());
        return new FileInfo(path);
    }

    static void RunSamples(string samplesProjectDir, FileInfo allureConfigFile)
    {
        var configuration = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyConfigurationAttribute>()
            ?.Configuration
            ?? "Debug";
        var dotnetTestProcessInfo = new ProcessStartInfo
        {
            WorkingDirectory = samplesProjectDir,
            FileName = "dotnet",
            Arguments = $"test --configuration {configuration}",
        };
        dotnetTestProcessInfo.Environment[AllureConstants.ALLURE_CONFIG_ENV_VARIABLE] =
            allureConfigFile.FullName;
        Process.Start(dotnetTestProcessInfo)?.WaitForExit();
    }

    [TestCase(Status.passed)]
    [TestCase(Status.failed)]
    [TestCase(Status.broken)]
    [TestCase(Status.skipped)]
    public void TestStatus(Status status)
    {
        var expected = scenariosByStatus![status.ToString()];
        var actual = results!.Where(x => x.status == status).Select(x => x.name).ToList();
        Assert.That(actual, Is.EquivalentTo(expected));
    }

    [Test]
    public void ShouldConvertTableToStepParams()
    {
        var parameters = results!
            .First(x => x.name == "Table arguments")
            .steps
            .SelectMany(s => s.parameters);

        Assert.That(parameters.Select(x => x.name), Has.Exactly(1).EqualTo("name"));
        Assert.That(parameters.Select(x => x.name), Has.Exactly(1).EqualTo("surname"));
        Assert.That(parameters.Select(x => x.name), Has.Exactly(2).EqualTo("width"));
        Assert.That(parameters.Select(x => x.name), Has.Exactly(0).EqualTo("attribute"));
    }

    [Test]
    public void ShouldNotDuplicateAfterFixtures()
    {
        var afters = containers!.Select(x => x.afters.Select(y => y.name));
        Assert.That(afters, Is.All.Unique);
    }

    [Test]
    public void ShouldNotDuplicateBeforeFixtures()
    {
        var befores = containers!.Select(x => x.befores.Select(y => y.name));
        Assert.That(befores, Is.All.Unique);
    }

    [Test]
    public void ShouldParseLinks()
    {
        var scenarios = results!
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
        var scenarios = results!
            .Where(x => x.labels.Any(l => l.value == "labels"));
        var labels = scenarios.SelectMany(x => x.labels);

        Assert.Multiple(() =>
        {
            var tags = labels.Where(x => x.name == "tag")
                .Select(l => l.value)
                .ToList();
            // all selected scenarios should have only 2 unmatched tags - "labels" and "passed". One scenario also has "tag1" as unmatched.
            Assert.That(
                tags,
                Has.Exactly(scenarios.Count() * 2 + 1).Items,
                string.Join(", ", tags)
            );

            // owner
            var owners = labels.Where(x => x.name == "owner")
                .Select(l => l.value)
                .ToList();
            Assert.That(
                owners,
                Has.Exactly(scenarios.Count()).Items.And.All.EqualTo("JohnDoe"),
                string.Join(", ", owners)
            );
        });
    }

    [Test]
    public void ShouldParseCustomLabel()
    {
        var scenarios = results!
          .Where(x => x.labels.Any(l => l.value == "labels"));

        var labels = scenarios.SelectMany(x => x.labels);

        Assert.That(labels.Where(x => x.value == "pepa").Select(l => l.name),
          Has.Exactly(1).Items.And.All.EqualTo("custom_label"));
    }

    [Test]
    public void ShouldAddParametersForScenarioExamples()
    {
        var parameters = results!
            .Where(x => x.name == "Scenario with examples")
            .SelectMany(x => x.parameters.Select(p => (p.name, p.value)))
            .ToArray();

        Assert.That(
            parameters,
            Has
                .Exactly(1).Items.Matches<(string name, string value)>(
                    x => x.name == "id" && x.value == "\"1\""
                )
                .And.Exactly(1).Items.Matches<(string name, string value)>(
                    x => x.name == "name" && x.value == "\"John\""
                )
        );

        Assert.That(
            parameters,
            Has
                .Exactly(1).Items.Matches<(string name, string value)>(
                    x => x.name == "id" && x.value == "\"2\""
                )
                .And.Exactly(1).Items.Matches<(string name, string value)>(
                    x => x.name == "name" && x.value == "\"Alex\""
                )
        );
    }

    [Test]
    public void ShouldReadHostNameFromConfigTitle()
    {
        var hostNames = results!
            .SelectMany(
                x => x.labels
                    .Where(l => l.name == "host")
                    .Select(l => l.value)
            )
            .Distinct();

        Assert.That(hostNames, Has.One.Items.And.All.EqualTo("5994A3F7-AF84-46AD-9393-000BB45553CC"));
    }

    static List<T> ParseResultFiles<T>(string rsultsDir, string pattern) =>
        new DirectoryInfo(rsultsDir).GetFiles(pattern).Select(
            f => JsonConvert.DeserializeObject<T>(
                    File.ReadAllText(f.FullName)
                )
                ?? throw new InvalidOperationException(
                    $"Unable to parse {f.FullName}"
                )
        ).ToList();

    static Dictionary<string, List<string>> ParseFeatures(string featuresDir)
    {
        var parser = new Parser();
        var scenarios = new List<Scenario>();
        var features = new DirectoryInfo(featuresDir).GetFiles("*.feature");
        scenarios.AddRange(
            features.SelectMany(f =>
            {
                var children = parser.Parse(f.FullName).Feature.Children.ToList();
                var scenarioOutlines = children.Where(
                    x => (x as dynamic).Examples.Length > 0
                ).ToList();
                foreach (var s in scenarioOutlines)
                {
                    var examplesCount = (s as dynamic).Examples[0]
                        .TableBody.Length;
                    for (int i = 1; i < examplesCount; i++)
                    {
                        children.Add(s);
                    }
                }
                return children.Select(
                    c => c as Scenario
                        ?? throw new InvalidOperationException($"Can't parse {f.FullName}")
                );
            })
        );

        var scenariosByStatus = scenarios.GroupBy(
            x => x.Tags.FirstOrDefault(
                x => Enum.GetNames(
                    typeof(Status)
                ).Contains(
                    x.Name.Replace("@", "")
                )
            )?.Name.Replace("@", "") ?? "_notag_",
            x => x.Name
          ).ToDictionary(g => g.Key, g => g.ToList());

        // Extra placeholder scenario for testing an exception in AfterFeature
        scenariosByStatus["broken"].Add(
            "AfterFeature of 'After Feature Failure' has failed"
        );
        return scenariosByStatus;
    }
}
