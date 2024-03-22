using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Allure.ReqnrollPlugin.Configuration;
using Newtonsoft.Json;
using NUnit.Framework;
using Reqnroll;

namespace Allure.ReqnrollPlugin.Tests.Unit;

class ConfigurationTests
{
    [Test]
    public void DefaultConfig()
    {
        var config = new AllureReqnrollConfiguration();

        Assert.That(config.RunnerType, Is.SameAs(typeof(TestRunner)));
        Assert.That(
            config.GherkinPatterns.StepArguments,
            Is.EqualTo(new DataTableToArgsConversionOptions(false, null, null))
        );
        Assert.That(
            config.IgnoreExceptions,
            Is.EquivalentTo(new[]
            {
                typeof(IgnoreException).FullName,
                "Xunit.SkipException"
            })
        );
        
        var patterns = config.GherkinPatterns;
        var grouping = patterns.Grouping;

        var suites = grouping.Suites;
        AssertRegexMatchWholeCi(suites.ParentSuite, "allure.parentSuite:p", "p");
        AssertRegexMatchWholeCi(suites.Suite, "allure.suite:s", "s");
        AssertRegexMatchWholeCi(suites.SubSuite, "allure.subSuite:s", "s");

        var bdd = grouping.Behaviors;
        AssertRegexMatchWholeCi(bdd.Epic, "allure.epic:e", "e");
        AssertRegexMatchWholeCi(bdd.Story, "allure.story:s", "s");

        var metadata = patterns.Metadata;
        AssertRegexMatchWholeCi(metadata.Owner, "allure.owner:o", "o");
        AssertRegexMatchWholeCi(metadata.Severity, "trivial", "trivial");
        AssertRegexMatchWholeCi(metadata.Severity, "minor", "minor");
        AssertRegexMatchWholeCi(metadata.Severity, "normal", "normal");
        AssertRegexMatchWholeCi(metadata.Severity, "critical", "critical");
        AssertRegexMatchWholeCi(metadata.Severity, "blocker", "blocker");
        AssertRegexMatchWholeCi(metadata.Label, "allure.label.layer:l", "layer", "l");

        var links = patterns.Links;
        AssertRegexMatchWholeCi(links.Link, "allure.link:https://allurereport.org/", "https://allurereport.org/");
        AssertRegexMatchWholeCi(links.Issue, "allure.issue:453", "453");
        AssertRegexMatchWholeCi(links.Tms, "allure.tms:453", "453");
    }

    [Test]
    public void CustomConfig()
    {
        var config = AllureReqnrollConfiguration.ParseConfig(
            GenerateJsonForConfig()
        );

        Assert.That(config.RunnerType, Is.SameAs(typeof(string)));
        Assert.That(config.IgnoreExceptions, Is.EqualTo(new[] { "MyException" }));

        var patterns = config.GherkinPatterns;

        var stepArgs = patterns.StepArguments;
        Assert.That(stepArgs.CreateFromDataTables, Is.True);
        AssertRegexMatchWholeCi(stepArgs.NameColumn, "my-name");
        AssertRegexMatchWholeCi(stepArgs.ValueColumn, "my-value");

        var grouping = patterns.Grouping;

        var suites = grouping.Suites;
        AssertRegexMatchWholeCi(suites.ParentSuite, "my-parent-suite=p", "p");
        AssertRegexMatchWholeCi(suites.Suite, "my-suite=s", "s");
        AssertRegexMatchWholeCi(suites.SubSuite, "my-sub-suite=s", "s");

        var bdd = grouping.Behaviors;
        AssertRegexMatchWholeCi(bdd.Epic, "my-epic=e", "e");
        AssertRegexMatchWholeCi(bdd.Story, "my-story=s", "s");

        var metadata = patterns.Metadata;
        AssertRegexMatchWholeCi(metadata.Owner, "owner=o", "o");
        AssertRegexMatchWholeCi(metadata.Severity, "my-severity=s", "s");
        AssertRegexMatchWholeCi(metadata.Label, "my-label=layer:l", "layer", "l");

        var links = patterns.Links;
        AssertRegexMatchWholeCi(links.Link, "my-link=https://allurereport.org/", "https://allurereport.org/");
        AssertRegexMatchWholeCi(links.Issue, "my-issue=453", "453");
        AssertRegexMatchWholeCi(links.Tms, "my-tms=453", "453");
    }

    [Test]
    public void CustomConfigWithJsPatternSyntax()
    {
        var config = AllureReqnrollConfiguration.ParseConfig(
            GenerateJsonForConfig(
                nameColumn: "/my-name/",
                valueColumn: "/my-value/",
                parentSuite: "/my-parent-suite=(.+)/",
                suite: "/my-suite=(.+)/",
                subSuite: "/my-sub-suite=(.+)/",
                epic: "/my-epic=(.+)/",
                story: "/my-story=(.+)/",
                owner: "/owner=(.+)/",
                severity: "/my-severity=(.+)/",
                label: @"/my-label=(\w+):(.+)/",
                link: "/my-link=(.*)/",
                issue: "/my-issue=(.*)/",
                tms: "/my-tms=(.*)/"
            )
        );

        Assert.That(config.RunnerType, Is.SameAs(typeof(string)));

        var patterns = config.GherkinPatterns;

        var stepArgs = patterns.StepArguments;
        Assert.That(stepArgs.CreateFromDataTables, Is.True);
        AssertRegexMatchFragmentCs(stepArgs.NameColumn, "my-name");
        AssertRegexMatchFragmentCs(stepArgs.ValueColumn, "my-value");

        var grouping = patterns.Grouping;

        var suites = grouping.Suites;
        AssertRegexMatchFragmentCs(suites.ParentSuite, "my-parent-suite=p", "p");
        AssertRegexMatchFragmentCs(suites.Suite, "my-suite=s", "s");
        AssertRegexMatchFragmentCs(suites.SubSuite, "my-sub-suite=s", "s");

        var bdd = grouping.Behaviors;
        AssertRegexMatchFragmentCs(bdd.Epic, "my-epic=e", "e");
        AssertRegexMatchFragmentCs(bdd.Story, "my-story=s", "s");

        var metadata = patterns.Metadata;
        AssertRegexMatchFragmentCs(metadata.Owner, "owner=o", "o");
        AssertRegexMatchFragmentCs(metadata.Severity, "my-severity=s", "s");
        AssertRegexMatchFragmentCs(metadata.Label, "my-label=layer:l", "layer", "l");

        var links = patterns.Links;
        AssertRegexMatchFragmentCs(links.Link, "my-link=https://allurereport.org/", "https://allurereport.org/");
        AssertRegexMatchFragmentCs(links.Issue, "my-issue=453", "453");
        AssertRegexMatchFragmentCs(links.Tms, "my-tms=453", "453");
    }

    static string GenerateJsonForConfig(
        bool createFromDataTables = true,
        string nameColumn = "my-name",
        string valueColumn = "my-value",
        string parentSuite = "my-parent-suite=(.+)",
        string suite = "my-suite=(.+)",
        string subSuite = "my-sub-suite=(.+)",
        string epic = "my-epic=(.+)",
        string story = "my-story=(.+)",
        string owner = "owner=(.+)",
        string severity = "my-severity=(.+)",
        string label = @"my-label=(\w+):(.+)",
        string link = "my-link=(.*)",
        string issue = "my-issue=(.*)",
        string tms = "my-tms=(.*)",
        string runnerType = "System.String",
        List<string>? ignoreExceptions = null
    ) => JsonConvert.SerializeObject(
        new
        {
            allure = new
            {
                gherkinPatterns = new
                {
                    stepArguments = new
                    {
                        createFromDataTables,
                        nameColumn,
                        valueColumn
                    },
                    grouping = new
                    {
                        suites = new { parentSuite, suite, subSuite },
                        behaviors = new { epic, story }
                    },
                    metadata = new { owner, severity, label },
                    links = new { link, issue, tms }
                },
                runnerType,
                ignoreExceptions = ignoreExceptions ?? new() { "MyException" }
            }
        }
    );

    static void AssertRegexMatchWholeCi(Regex? pattern, string value, params string[] groups)
    {
        AssertRegexMatch(pattern, value, groups);
        AssertRegexNotMatchFragment(pattern, value);
        AssertMatchCaseInsensitive(pattern, value);
    }

    static void AssertRegexMatchFragmentCs(Regex? pattern, string value, params string[] groups)
    {
        AssertRegexMatch(pattern, value, groups);
        AssertRegexMatchFragment(pattern, value);
        AssertMatchCaseSensitive(pattern, value);
    }

    static void AssertRegexMatch(Regex? pattern, string value, params string[] groups)
    {
        Assert.That(pattern, Is.Not.Null);
        var match = pattern!.Match(value);
        Assert.That(match.Success, Is.True);
        Assert.That(match.Groups.Count, Is.EqualTo(groups.Length + 1));

        Assert.That(
            match.Groups.Cast<Group>().Skip(1).Select(g => g.Value),
            Is.EqualTo(groups)
        );
    }

    static void AssertMatchCaseInsensitive(Regex? pattern, string value)
    {
        Assert.That(pattern?.IsMatch(value.ToUpper()), Is.True);
    }

    static void AssertMatchCaseSensitive(Regex? pattern, string value)
    {
        Assert.That(pattern?.IsMatch(value.ToUpper()), Is.False);
    }

    static void AssertRegexMatchFragment(Regex? pattern, string value)
    {
        Assert.That(pattern?.IsMatch($"-{value}-"), Is.True);
    }

    static void AssertRegexNotMatchFragment(Regex? pattern, string value)
    {
        Assert.That(pattern?.IsMatch($"-{value}-"), Is.False);
    }
}
