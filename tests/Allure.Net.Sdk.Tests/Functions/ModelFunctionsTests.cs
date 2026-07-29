using System.Collections;
using System.Collections.Immutable;
using Allure.Model;
using Allure.Sdk.Configuration;
using Allure.Sdk.Functions;
using AllureTestResult = Allure.Model.TestResult;

namespace Allure.Net.Sdk.Tests.Functions;

public class ModelFunctionsTests
{
    [Test]
    public async Task ShouldClassifyKnownExceptionTypesAndInterfaces()
    {
        var exception = new DerivedException();

        await Assert.That(ErrorStatus.IsKnown([typeof(BaseException).FullName!], exception)).IsTrue();
        await Assert.That(ErrorStatus.IsKnown([typeof(IMarker).FullName!], exception)).IsTrue();
        await Assert.That(ErrorStatus.IsKnown([typeof(InvalidOperationException).FullName!], exception)).IsFalse();
        await Assert.That(ErrorStatus.Resolve([], exception)).IsEqualTo(Status.Broken);
        await Assert.That(ErrorStatus.Resolve([typeof(BaseException).FullName!], exception))
            .IsEqualTo(Status.Failed);
    }

    [Test]
    public async Task ShouldCreateGlobalLabelsFromConfiguration()
    {
        var configuration = new AllureConfiguration
        {
            GlobalLabels = ImmutableDictionary<string, string>.Empty
                .Add("browser", "Chrome")
                .Add("", "ignored")
                .Add("empty", ""),
        };

        var labels = GlobalLabels.FromConfiguration(configuration).ToList();

        var label = await Assert.That(labels).HasSingleItem();
        await Assert.That(label.Name).IsEqualTo("browser");
        await Assert.That(label.Value).IsEqualTo("Chrome");
    }

    [Test]
    public async Task ShouldCreateGlobalLabelsFromConfigurationAndEnvironmentValues()
    {
        var environment = new Hashtable
        {
            ["ALLURE_LABEL_os"] = "macOS",
            ["ALLURE_LABEL_"] = "ignored",
            ["allure_LABEL_case"] = "ignored",
            [42] = "ignored",
        };

        var labels = GlobalLabels.FromEnvironmentVariables(environment).ToList();

        var label = await Assert.That(labels).HasSingleItem();
        await Assert.That(label.Name).IsEqualTo("os");
        await Assert.That(label.Value).IsEqualTo("macOS");
    }

    [Test]
    public async Task ShouldApplyLinkTemplateToRelativeUrls()
    {
        var templates = ImmutableDictionary<string, AllureLinkTemplate>.Empty
            .Add("issue", new("https://tracker/{0}", "Issue {0}"));
        var link = new Link { Type = "issue", Url = "123" };

        LinkTemplates.Apply(templates, link);

        await Assert.That(link.Url).IsEqualTo("https://tracker/123");
        await Assert.That(link.Name).IsEqualTo("Issue 123");
    }

    [Test]
    public async Task ShouldKeepTheLinkNameIfProvided()
    {
        var templates = ImmutableDictionary<string, AllureLinkTemplate>.Empty
            .Add("issue", new("https://tracker/{0}", "Issue {0}"));
        var link = new Link { Type = "issue", Url = "123", Name = "Foo" };

        LinkTemplates.Apply(templates, link);

        await Assert.That(link.Name).IsEqualTo("Foo");
    }

    [Test]
    public async Task ShouldNotApplyLinkTemplateToAbsoluteUrls()
    {
        var templates = ImmutableDictionary<string, AllureLinkTemplate>.Empty
            .Add("issue", new("https://tracker/{0}", "Issue {0}"));
        var link = new Link { Type = "issue", Url = "https://example.org/123", Name = "Original" };

        LinkTemplates.Apply(templates, link);

        await Assert.That(link.Url).IsEqualTo("https://example.org/123");
        await Assert.That(link.Name).IsEqualTo("Original");
    }

    [Test]
    public async Task ShouldCreateStatusDetailsFromException()
    {
        var exception = new Exception("");
        var details = StatusDetails.FromException(exception);

        await Assert.That(StatusDetails.FromException(null)).IsNull();
        await Assert.That(details!.Message).IsEqualTo(nameof(Exception));
        await Assert.That(details.Trace).IsEqualTo(exception.ToString());
    }

    [Test]
    public async Task ShouldAddDefaultSuiteLabelsOnlyOnce()
    {
        var test = new AllureTestResult { Uuid = Guid.NewGuid().ToString(), Name = "test" };

        SuiteLabels.Ensure(test, "parent", "suite", "sub");
        SuiteLabels.Ensure(test, "other", "other", "other");

        await Assert.That(test.Labels.Select(label => label.Value))
            .IsEquivalentTo(["parent", "suite", "sub"], TUnit.Assertions.Enums.CollectionOrdering.Matching);
    }

    interface IMarker;
    class BaseException : Exception;
    class DerivedException : BaseException, IMarker;
}
