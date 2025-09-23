using NUnit.Framework;
using Allure.Net.Commons.Functions;

namespace Allure.Net.Commons.Tests.FunctionTests.ModelFunctionTests;

class DefaultSuiteTests
{
    [Test]
    public void DefaultSuiteLabelsAdded()
    {
        TestResult testResult = new() { labels = [] };

        ModelFunctions.EnsureSuites(testResult, "foo", "bar", "baz");

        Assert.That(
            testResult.labels,
            Does.Contain(
                Label.ParentSuite("foo")
            ).UsingPropertiesComparer().And.Contains(
                Label.Suite("bar")
            ).UsingPropertiesComparer().And.Contains(
                Label.SubSuite("baz")
            ).UsingPropertiesComparer()
        );
    }

    [TestCase(null)]
    [TestCase("")]
    public void EmptyOrNullParentSuiteNotAdded(string parentSuite)
    {
        TestResult testResult = new() { labels = [] };

        ModelFunctions.EnsureSuites(testResult, parentSuite, "bar", "baz");

        Assert.That(
            testResult.labels,
            Has.Exactly(0).Matches<Label>(l => l.name == LabelName.PARENT_SUITE)
                .And.Contains(
                    Label.Suite("bar")
                ).UsingPropertiesComparer().And.Contains(
                    Label.SubSuite("baz")
                ).UsingPropertiesComparer()
        );
    }

    [TestCase(null)]
    [TestCase("")]
    public void EmptyOrNullSuiteNotAdded(string suite)
    {
        TestResult testResult = new() { labels = [] };

        ModelFunctions.EnsureSuites(testResult, "foo", suite, "baz");

        Assert.That(
            testResult.labels,
            Does.Contain(
                Label.ParentSuite("foo")
            ).UsingPropertiesComparer()
                .And.Exactly(0).Matches<Label>(l => l.name == LabelName.SUITE)
                .And.Contains(
                    Label.SubSuite("baz")
                ).UsingPropertiesComparer()
        );
    }

    [TestCase(null)]
    [TestCase("")]
    public void EmptyOrNullSubSuiteNotAdded(string subSuite)
    {
        TestResult testResult = new() { labels = [] };

        ModelFunctions.EnsureSuites(testResult, "foo", "bar", subSuite);

        Assert.That(
            testResult.labels,
            Does.Contain(
                Label.ParentSuite("foo")
            ).UsingPropertiesComparer().And.Contains(
                Label.Suite("bar")
            ).UsingPropertiesComparer().And.Exactly(0).Matches<Label>(
                l => l.name == LabelName.SUB_SUITE
            )
        );
    }

    [TestCase("parentSuite")]
    [TestCase("suite")]
    [TestCase("subSuite")]
    public void DefaultSuiteLabelsNotAddedIfSuitesHierarchyDefined(string labelName)
    {
        TestResult testResult = new() { labels = [new() { name = labelName, value = "qux" }] };

        ModelFunctions.EnsureSuites(testResult, "foo", "bar", "baz");

        Assert.That(
            testResult.labels,
            Does.Not.Contain(Label.ParentSuite("foo")).UsingPropertiesComparer()
                .And.Not.Contains(Label.Suite("bar")).UsingPropertiesComparer()
                .And.Not.Contains(Label.SubSuite("baz")).UsingPropertiesComparer()
        );
    }
}