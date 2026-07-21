using Allure.Model;
using ModelTestResult = Allure.Model.TestResult;

namespace Allure.Net.Tests.Model;

public class ModelContractTests
{
    [Test]
    public async Task ExecutableItemCollectionsAreOwnedByEachInstance()
    {
        var first = CreateTestResult("first");
        var second = CreateTestResult("second");

        first.Steps.Add(new() { Name = "step" });
        first.Attachments.Add(new()
        {
            Name = "attachment",
            Source = "source",
            FileExtension = ".txt",
        });
        first.Parameters.Add(new() { Name = "parameter", Value = "value" });

        await Assert.That(second.Steps).IsEmpty();
        await Assert.That(second.Attachments).IsEmpty();
        await Assert.That(second.Parameters).IsEmpty();
        await Assert.That(ReferenceEquals(first.Steps, second.Steps)).IsFalse();
        await Assert.That(ReferenceEquals(first.Attachments, second.Attachments)).IsFalse();
        await Assert.That(ReferenceEquals(first.Parameters, second.Parameters)).IsFalse();
    }

    [Test]
    public async Task TestResultCollectionsAreOwnedByEachInstance()
    {
        var first = CreateTestResult("first");
        var second = CreateTestResult("second");

        first.TitlePath.Add("title");
        first.Labels.Add(Label.Tag("tag"));
        first.Links.Add(new() { Url = "https://example.test" });

        await Assert.That(second.TitlePath).IsEmpty();
        await Assert.That(second.Labels).IsEmpty();
        await Assert.That(second.Links).IsEmpty();
        await Assert.That(ReferenceEquals(first.TitlePath, second.TitlePath)).IsFalse();
        await Assert.That(ReferenceEquals(first.Labels, second.Labels)).IsFalse();
        await Assert.That(ReferenceEquals(first.Links, second.Links)).IsFalse();
    }

    [Test]
    public async Task ScopeCollectionsAreOwnedByEachInstance()
    {
        var first = new Scope { Uuid = "first", Name = null };
        var second = new Scope { Uuid = "second", Name = "scope" };

        first.Children.Add("test-id");
        first.Befores.Add(new() { Name = "before" });
        first.Afters.Add(new() { Name = "after" });

        await Assert.That(second.Children).IsEmpty();
        await Assert.That(second.Befores).IsEmpty();
        await Assert.That(second.Afters).IsEmpty();
        await Assert.That(ReferenceEquals(first.Children, second.Children)).IsFalse();
        await Assert.That(ReferenceEquals(first.Befores, second.Befores)).IsFalse();
        await Assert.That(ReferenceEquals(first.Afters, second.Afters)).IsFalse();
    }

    [Test]
    public async Task GlobalsCollectionsAreOwnedByEachInstance()
    {
        var first = new Globals();
        var second = new Globals();

        first.Attachments.Add(new()
        {
            Name = "attachment",
            Source = "source",
            FileExtension = ".txt",
        });

        await Assert.That(second.Attachments).IsEmpty();
        await Assert.That(second.Errors).IsEmpty();
        await Assert.That(ReferenceEquals(first.Attachments, second.Attachments)).IsFalse();
        await Assert.That(ReferenceEquals(first.Errors, second.Errors)).IsFalse();
    }

    [Test]
    public async Task GlobalsErrorsCollectionStoresGlobalErrors()
    {
        var property = typeof(Globals).GetProperty(nameof(Globals.Errors))!;

        await Assert.That(property.PropertyType)
            .IsEqualTo(typeof(List<GlobalError>));
    }

    [Test]
    public async Task ModelDefaultsRepresentAnUnstartedExecutableItem()
    {
        var result = CreateTestResult("test");

        await Assert.That(result.Status).IsEqualTo(Status.Unknown);
        await Assert.That(result.Stage).IsEqualTo(Stage.Scheduled);
        await Assert.That(result.StatusDetails).IsNull();
        await Assert.That(result.Description).IsNull();
        await Assert.That(result.DescriptionHtml).IsNull();
        await Assert.That(result.Start).IsEqualTo(0);
        await Assert.That(result.Stop).IsEqualTo(0);
    }

    [Test]
    public async Task SpecializedModelsHaveExpectedBaseTypes()
    {
        await Assert.That(typeof(StepResult).BaseType).IsEqualTo(typeof(ExecutableItem));
        await Assert.That(typeof(FixtureResult).BaseType).IsEqualTo(typeof(ExecutableItem));
        await Assert.That(typeof(ModelTestResult).BaseType).IsEqualTo(typeof(ExecutableItem));
        await Assert.That(typeof(GlobalAttachment).BaseType).IsEqualTo(typeof(Attachment));
        await Assert.That(typeof(GlobalError).BaseType).IsEqualTo(typeof(StatusDetails));
    }

    static ModelTestResult CreateTestResult(string id) => new()
    {
        Uuid = id,
        Name = id,
    };
}
