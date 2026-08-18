using System.Text.Json;
using Allure.Model;
using Allure.Sdk.Results;

namespace Allure.TestingPlatform.Tests.OperationTests;

public abstract class OperationTestsBase
{
    protected abstract Task AddAttachment(
        string name,
        Stream content,
        string mediaType,
        string fileExtension
    );

    protected abstract Task AddAttachmentFromFile(
        string path,
        string name,
        string mediaType,
        string fileExtension
    );

    protected abstract Task AddGlobalAttachment(
        string name,
        Stream content,
        string mediaType,
        string fileExtension
    );

    protected abstract Task AddGlobalAttachmentFromFile(
        string path,
        string name,
        string mediaType,
        string fileExtension
    );

    protected abstract Task AddGlobalError(StatusDetails statusDetails);

    protected abstract Task AddLabel(Label label);

    protected abstract Task AddLabels(IEnumerable<Label> labels);

    protected abstract Task AddLink(Link link);

    protected abstract Task AddLinks(IEnumerable<Link> links);

    protected abstract Task AddScreenDiff(
        Stream expected,
        Stream actual,
        Stream diff
    );

    protected abstract Task AddScreenDiffFromFiles(
        string expectedPath,
        string actualPath,
        string diffPath
    );

    protected abstract Task AddTestParameter(Parameter parameter);

    protected abstract Task SetDescription(string description);

    protected abstract Task SetDescriptionHtml(string descriptionHtml);

    protected abstract Task SetFixtureName(string newName);

    protected abstract Task SetLabel(string value);

    protected abstract Task SetName(string newName);

    protected abstract Task SetTestName(string newName);

    protected abstract Task SetUpAction(string name, Action body);

    protected abstract Task<int> SetUpFunction(string name, Func<int> body);

    protected abstract Task StepCompleted(
        string name,
        Status status,
        StatusDetails statusDetails
    );

    protected abstract Task StepAction(string name, Action body);

    protected abstract Task<int> StepFunction(string name, Func<int> body);

    protected abstract Task RunNestedSteps(string outerName, string innerName);

    protected abstract Task RunConcurrentSteps(params string[] names);

    protected abstract Task RunNestedConcurrentSteps(
        IReadOnlyDictionary<string, string[]> stepNames
    );

    protected abstract Task TearDownAction(string name, Action body);

    protected abstract Task<int> TearDownFunction(string name, Func<int> body);

    [Test]
    public async Task ShouldAddAttachment()
    {
        using MemoryStream content = new([1, 2, 3]);

        var destination = await Run(() => this.AddAttachment("text", content, "text/plain", ".txt"));

        var testResult = await Assert.That(destination.TestResults).HasSingleItem();
        var attachment = await Assert.That(testResult.Attachments).HasSingleItem();
        await Assert.That(attachment.Name).IsEqualTo("text");
        await Assert.That(attachment.Type).IsEqualTo("text/plain");
        await Assert.That(attachment.FileExtension).IsEqualTo(".txt");
        await Assert.That(destination.ByteAttachments).ContainsKey(attachment.Source);
        await Assert.That(destination.ByteAttachments[attachment.Source])
            .IsEquivalentTo(new byte[] { 1, 2, 3 });
    }

    [Test]
    public async Task ShouldAddAttachmentFromFile()
    {
        var path = await CreateFile([1, 2, 3]);

        try
        {
            var destination = await Run(
                () => this.AddAttachmentFromFile(
                    path,
                    "file attachment",
                    "application/octet-stream",
                    ".bin"
                )
            );

            var testResult = await Assert.That(destination.TestResults).HasSingleItem();
            var attachment = await Assert.That(testResult.Attachments).HasSingleItem();
            await Assert.That(attachment.Name).IsEqualTo("file attachment");
            await Assert.That(attachment.Type)
                .IsEqualTo("application/octet-stream");
            await Assert.That(attachment.FileExtension).IsEqualTo(".bin");
            await Assert.That(destination.FileAttachments).ContainsKey(attachment.Source);
            await Assert.That(destination.FileAttachments[attachment.Source])
                .IsEqualTo(path);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task ShouldAddAttachmentToCurrentStep()
    {
        using MemoryStream content = new([1, 2, 3]);

        var destination = await Run(
            () => this.AddAttachment(
                "step attachment",
                content,
                "application/octet-stream",
                ".bin"
            ),
            OperationTarget.Step
        );

        var testResult = await Assert.That(destination.TestResults).HasSingleItem();
        await Assert.That(testResult.Attachments).IsEmpty();
        var step = await Assert.That(testResult.Steps).HasSingleItem();
        var attachment = await Assert.That(step.Attachments).HasSingleItem();
        await Assert.That(attachment.Name).IsEqualTo("step attachment");
        await Assert.That(attachment.Type)
            .IsEqualTo("application/octet-stream");
        await Assert.That(attachment.FileExtension).IsEqualTo(".bin");
        await Assert.That(destination.ByteAttachments).ContainsKey(attachment.Source);
        await Assert.That(destination.ByteAttachments[attachment.Source])
            .IsEquivalentTo(new byte[] { 1, 2, 3 });
    }

    [Test]
    public async Task ShouldAddAttachmentFromFileToCurrentStep()
    {
        var path = await CreateFile([1, 2, 3]);

        try
        {
            var destination = await Run(
                () => this.AddAttachmentFromFile(
                    path,
                    "step file attachment",
                    "application/octet-stream",
                    ".bin"
                ),
                OperationTarget.Step
            );

            var testResult = await Assert.That(destination.TestResults).HasSingleItem();
            await Assert.That(testResult.Attachments).IsEmpty();
            var step = await Assert.That(testResult.Steps).HasSingleItem();
            var attachment = await Assert.That(step.Attachments).HasSingleItem();
            await Assert.That(attachment.Name).IsEqualTo("step file attachment");
            await Assert.That(attachment.Type)
                .IsEqualTo("application/octet-stream");
            await Assert.That(attachment.FileExtension).IsEqualTo(".bin");
            await Assert.That(destination.FileAttachments).ContainsKey(attachment.Source);
            await Assert.That(destination.FileAttachments[attachment.Source])
                .IsEqualTo(path);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task ShouldAddAttachmentToCurrentFixture()
    {
        using MemoryStream content = new([1, 2, 3]);

        var destination = await Run(
            () => this.AddAttachment(
                "fixture attachment",
                content,
                "application/octet-stream",
                ".bin"
            ),
            OperationTarget.Fixture
        );

        var testResult = await Assert.That(destination.TestResults).HasSingleItem();
        await Assert.That(testResult.Attachments).IsEmpty();
        var container = await Assert.That(destination.TestContainers).HasSingleItem();
        var fixture = await Assert.That(container.Befores).HasSingleItem();
        var attachment = await Assert.That(fixture.Attachments).HasSingleItem();
        await Assert.That(attachment.Name).IsEqualTo("fixture attachment");
        await Assert.That(attachment.Type)
            .IsEqualTo("application/octet-stream");
        await Assert.That(attachment.FileExtension).IsEqualTo(".bin");
        await Assert.That(destination.ByteAttachments).ContainsKey(attachment.Source);
        await Assert.That(destination.ByteAttachments[attachment.Source])
            .IsEquivalentTo(new byte[] { 1, 2, 3 });
    }

    [Test]
    public async Task ShouldAddAttachmentFromFileToCurrentFixture()
    {
        var path = await CreateFile([1, 2, 3]);

        try
        {
            var destination = await Run(
                () => this.AddAttachmentFromFile(
                    path,
                    "fixture file attachment",
                    "application/octet-stream",
                    ".bin"
                ),
                OperationTarget.Fixture
            );

            var testResult = await Assert.That(destination.TestResults).HasSingleItem();
            await Assert.That(testResult.Attachments).IsEmpty();
            var container = await Assert.That(destination.TestContainers).HasSingleItem();
            var fixture = await Assert.That(container.Befores).HasSingleItem();
            var attachment = await Assert.That(fixture.Attachments).HasSingleItem();
            await Assert.That(attachment.Name).IsEqualTo("fixture file attachment");
            await Assert.That(attachment.Type)
                .IsEqualTo("application/octet-stream");
            await Assert.That(attachment.FileExtension).IsEqualTo(".bin");
            await Assert.That(destination.FileAttachments).ContainsKey(attachment.Source);
            await Assert.That(destination.FileAttachments[attachment.Source])
                .IsEqualTo(path);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task ShouldAddGlobalAttachment()
    {
        using MemoryStream content = new([1, 2, 3]);

        var destination = await Run(
            () => this.AddGlobalAttachment(
                "global attachment",
                content,
                "application/octet-stream",
                ".bin"
            )
        );

        var globals = await Assert.That(destination.Globals).HasSingleItem();
        var attachment = await Assert.That(globals.Attachments).HasSingleItem();
        await Assert.That(attachment.Name).IsEqualTo("global attachment");
        await Assert.That(attachment.Type)
            .IsEqualTo("application/octet-stream");
        await Assert.That(attachment.FileExtension).IsEqualTo(".bin");
        await Assert.That(destination.ByteAttachments).ContainsKey(attachment.Source);
        await Assert.That(destination.ByteAttachments[attachment.Source])
            .IsEquivalentTo(new byte[] { 1, 2, 3 });
    }

    [Test]
    public async Task ShouldAddGlobalAttachmentFromFile()
    {
        var path = await CreateFile([1, 2, 3]);

        try
        {
            var destination = await Run(
                () => this.AddGlobalAttachmentFromFile(
                    path,
                    "global file attachment",
                    "application/octet-stream",
                    ".bin"
                )
            );

            var globals = await Assert.That(destination.Globals).HasSingleItem();
            var attachment = await Assert.That(globals.Attachments).HasSingleItem();
            await Assert.That(attachment.Name).IsEqualTo("global file attachment");
            await Assert.That(attachment.Type)
                .IsEqualTo("application/octet-stream");
            await Assert.That(attachment.FileExtension).IsEqualTo(".bin");
            await Assert.That(destination.FileAttachments).ContainsKey(attachment.Source);
            await Assert.That(destination.FileAttachments[attachment.Source])
                .IsEqualTo(path);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task ShouldAddGlobalError()
    {
        var destination = await Run(
            () => this.AddGlobalError(new()
            {
                Message = "error",
                Trace = "trace",
                Flaky = true,
                Known = true,
                Muted = true,
            })
        );

        var globals = await Assert.That(destination.Globals).HasSingleItem();
        var error = await Assert.That(globals.Errors).HasSingleItem();
        await Assert.That(error.Message).IsEqualTo("error");
        await Assert.That(error.Trace).IsEqualTo("trace");
        await Assert.That(error.Flaky).IsTrue();
        await Assert.That(error.Known).IsTrue();
        await Assert.That(error.Muted).IsTrue();
    }

    [Test]
    public async Task ShouldAddLabel()
    {
        var destination = await Run(
            () => this.AddLabel(new() { Name = "label", Value = "value" })
        );

        var testResult = await Assert.That(destination.TestResults).HasSingleItem();
        var label = testResult.Labels.Single(label => label.Name == "label");
        await Assert.That(label.Name).IsEqualTo("label");
        await Assert.That(label.Value).IsEqualTo("value");
    }

    [Test]
    public async Task ShouldAddLabels()
    {
        Label[] labels =
        [
            new() { Name = "first", Value = "one" },
            new() { Name = "second", Value = "two" },
        ];

        var destination = await Run(() => this.AddLabels(labels));

        var testResult = await Assert.That(destination.TestResults).HasSingleItem();
        await Assert.That(
            testResult.Labels
                .Where(label => label.Name is "first" or "second")
                .Select(label => $"{label.Name}:{label.Value}")
        ).IsEquivalentTo(["first:one", "second:two"]);
    }

    [Test]
    public async Task ShouldAddLink()
    {
        var destination = await Run(
            () => this.AddLink(new()
            {
                Name = "link",
                Type = "issue",
                Url = "https://example.org",
            })
        );

        var testResult = await Assert.That(destination.TestResults).HasSingleItem();
        var link = await Assert.That(testResult.Links).HasSingleItem();
        await Assert.That(link.Name).IsEqualTo("link");
        await Assert.That(link.Type).IsEqualTo("issue");
        await Assert.That(link.Url).IsEqualTo("https://example.org");
    }

    [Test]
    public async Task ShouldAddLinks()
    {
        Link[] links =
        [
            new()
            {
                Name = "first",
                Type = "issue",
                Url = "https://example.org/1",
            },
            new()
            {
                Name = "second",
                Type = "tms",
                Url = "https://example.org/2",
            },
        ];

        var destination = await Run(() => this.AddLinks(links));

        var testResult = await Assert.That(destination.TestResults).HasSingleItem();
        await Assert.That(testResult.Links).Count().IsEqualTo(2);
        await Assert.That(testResult.Links[0].Name).IsEqualTo("first");
        await Assert.That(testResult.Links[0].Type).IsEqualTo("issue");
        await Assert.That(testResult.Links[0].Url).IsEqualTo("https://example.org/1");
        await Assert.That(testResult.Links[1].Name).IsEqualTo("second");
        await Assert.That(testResult.Links[1].Type).IsEqualTo("tms");
        await Assert.That(testResult.Links[1].Url).IsEqualTo("https://example.org/2");
    }

    [Test]
    public async Task ShouldAddScreenDiff()
    {
        using MemoryStream expected = new([1]);
        using MemoryStream actual = new([2]);
        using MemoryStream diff = new([3]);

        var destination = await Run(
            () => this.AddScreenDiff(expected, actual, diff)
        );

        await AssertScreenDiff(
            destination,
            "data:image/png;base64,AQ==",
            "data:image/png;base64,Ag==",
            "data:image/png;base64,Aw=="
        );
    }

    [Test]
    public async Task ShouldAddScreenDiffFromFiles()
    {
        var expectedPath = await CreateFile([1], ".png");
        var actualPath = await CreateFile([2], ".png");
        var diffPath = await CreateFile([3], ".png");

        try
        {
            var destination = await Run(
                () => this.AddScreenDiffFromFiles(
                    expectedPath,
                    actualPath,
                    diffPath
                )
            );

            await AssertScreenDiff(
                destination,
                "data:image/png;base64,AQ==",
                "data:image/png;base64,Ag==",
                "data:image/png;base64,Aw=="
            );
        }
        finally
        {
            File.Delete(expectedPath);
            File.Delete(actualPath);
            File.Delete(diffPath);
        }
    }

    [Test]
    public async Task ShouldAddTestParameter()
    {
        var destination = await Run(
            () => this.AddTestParameter(new()
            {
                Name = "parameter",
                Value = "value",
                Mode = ParameterMode.Masked,
                Excluded = true,
            })
        );

        var testResult = await Assert.That(destination.TestResults).HasSingleItem();
        var parameter = await Assert.That(testResult.Parameters).HasSingleItem();
        await Assert.That(parameter.Name).IsEqualTo("parameter");
        await Assert.That(parameter.Value).IsEqualTo("value");
        await Assert.That(parameter.Mode).IsEqualTo(ParameterMode.Masked);
        await Assert.That(parameter.Excluded).IsTrue();
    }

    [Test]
    public async Task ShouldSetDescription()
    {
        var destination = await Run(
            () => this.SetDescription("description")
        );

        await Assert.That(destination.TestResults.Single().Description)
            .IsEqualTo("description");
    }

    [Test]
    public async Task ShouldSetDescriptionHtml()
    {
        var destination = await Run(
            () => this.SetDescriptionHtml("<p>description</p>")
        );

        await Assert.That(destination.TestResults.Single().DescriptionHtml)
            .IsEqualTo("<p>description</p>");
    }

    [Test]
    public async Task ShouldSetFixtureName()
    {
        var destination = await Run(
            () => this.SetFixtureName("new fixture name"),
            OperationTarget.Fixture
        );

        await Assert.That(destination.TestContainers.Single().Befores.Single().Name)
            .IsEqualTo("new fixture name");
    }

    [Test]
    public async Task ShouldSetLabel()
    {
        var destination = await Run(() => this.SetLabel("owner"));

        await Assert.That(
            destination.TestResults.Single().Labels
                .Single(label => label.Name == LabelName.Owner)
                .Value
        ).IsEqualTo("owner");
    }

    [Test]
    public async Task ShouldSetName()
    {
        var destination = await Run(() => this.SetName("new name"));

        await Assert.That(destination.TestResults.Single().Name)
            .IsEqualTo("new name");
    }

    [Test]
    public async Task ShouldSetTestName()
    {
        var destination = await Run(() => this.SetTestName("new test name"));

        await Assert.That(destination.TestResults.Single().Name)
            .IsEqualTo("new test name");
    }

    [Test]
    public async Task ShouldRunSetUpAction()
    {
        var bodyCalled = false;
        var destination = await Run(
            () => this.SetUpAction("setup action", () => bodyCalled = true)
        );

        var container = await Assert.That(destination.TestContainers).HasSingleItem();
        var fixture = await Assert.That(container.Befores).HasSingleItem();
        await Assert.That(fixture.Name).IsEqualTo("setup action");
        await Assert.That(fixture.Status).IsEqualTo(Status.Passed);
        await Assert.That(bodyCalled).IsTrue();
    }

    [Test]
    public async Task ShouldRunSetUpFunction()
    {
        var result = 0;
        var destination = await Run(async () =>
        {
            result = await this.SetUpFunction("setup function", () => 42);
        });

        var container = await Assert.That(destination.TestContainers).HasSingleItem();
        var fixture = await Assert.That(container.Befores).HasSingleItem();
        await Assert.That(fixture.Name).IsEqualTo("setup function");
        await Assert.That(fixture.Status).IsEqualTo(Status.Passed);
        await Assert.That(result).IsEqualTo(42);
    }

    [Test]
    public async Task ShouldAddCompletedStep()
    {
        var destination = await Run(
            () => this.StepCompleted(
                "completed step",
                Status.Broken,
                new()
                {
                    Message = "broken",
                    Trace = "trace",
                    Flaky = true,
                    Known = true,
                    Muted = true,
                }
            )
        );

        var testResult = await Assert.That(destination.TestResults).HasSingleItem();
        var step = await Assert.That(testResult.Steps).HasSingleItem();
        await Assert.That(step.Name).IsEqualTo("completed step");
        await Assert.That(step.Status).IsEqualTo(Status.Broken);
        await Assert.That(step.StatusDetails).IsNotNull();
        await Assert.That(step.StatusDetails!.Message).IsEqualTo("broken");
        await Assert.That(step.StatusDetails.Trace).IsEqualTo("trace");
        await Assert.That(step.StatusDetails.Flaky).IsTrue();
        await Assert.That(step.StatusDetails.Known).IsTrue();
        await Assert.That(step.StatusDetails.Muted).IsTrue();
    }

    [Test]
    public async Task ShouldRunStepAction()
    {
        var bodyCalled = false;
        var destination = await Run(
            () => this.StepAction("step action", () => bodyCalled = true)
        );

        var testResult = await Assert.That(destination.TestResults).HasSingleItem();
        var step = await Assert.That(testResult.Steps).HasSingleItem();
        await Assert.That(step.Name).IsEqualTo("step action");
        await Assert.That(step.Status).IsEqualTo(Status.Passed);
        await Assert.That(bodyCalled).IsTrue();
    }

    [Test]
    public async Task ShouldRunStepFunction()
    {
        var result = 0;
        var destination = await Run(async () =>
        {
            result = await this.StepFunction("step function", () => 42);
        });

        var testResult = await Assert.That(destination.TestResults).HasSingleItem();
        var step = await Assert.That(testResult.Steps).HasSingleItem();
        await Assert.That(step.Name).IsEqualTo("step function");
        await Assert.That(step.Status).IsEqualTo(Status.Passed);
        await Assert.That(result).IsEqualTo(42);
    }

    [Test]
    public async Task ShouldRunNestedSteps()
    {
        var destination = await Run(
            () => this.RunNestedSteps("outer step", "inner step")
        );

        var testResult = await Assert.That(destination.TestResults).HasSingleItem();
        var outer = await Assert.That(testResult.Steps).HasSingleItem();
        await Assert.That(outer.Name).IsEqualTo("outer step");
        await Assert.That(outer.Status).IsEqualTo(Status.Passed);
        var inner = await Assert.That(outer.Steps).HasSingleItem();
        await Assert.That(inner.Name).IsEqualTo("inner step");
        await Assert.That(inner.Status).IsEqualTo(Status.Passed);
    }

    [Test]
    public async Task ShouldRunStepsConcurrently()
    {
        string[] names = [.. Enumerable.Range(1, 10).Select(i => $"Step {i}")];
        var destination = await Run(
            () => this.RunConcurrentSteps(names)
        );

        var testResult = await Assert.That(destination.TestResults).HasSingleItem();
        await Assert.That(testResult.Steps).Count().IsEqualTo(names.Length);
        await Assert.That(testResult.Steps.Select(step => step.Name))
            .IsEquivalentTo(names);
        await Assert.That(testResult.Steps.SelectMany(step => step.Steps))
            .IsEmpty();
        await Assert.That(testResult.Steps.Select(step => step.Status))
            .IsEquivalentTo(Enumerable.Repeat(Status.Passed, names.Length));
    }

    [Test]
    public async Task ShouldRunNestedStepsConcurrentlyWithoutInterleaving()
    {
        var stepNames = Enumerable.Range(1, 5).ToDictionary(
            parent => $"Step {parent}",
            parent => Enumerable.Range(1, 5)
                .Select(child => $"Step {parent}.{child}")
                .ToArray()
        );
        var destination = await Run(
            () => this.RunNestedConcurrentSteps(stepNames)
        );

        var testResult = await Assert.That(destination.TestResults).HasSingleItem();
        await Assert.That(testResult.Steps).Count().IsEqualTo(stepNames.Count);
        await Assert.That(testResult.Steps.Select(step => step.Name))
            .IsEquivalentTo(stepNames.Keys);

        foreach (var parent in testResult.Steps)
        {
            var expectedChildren = stepNames[parent.Name];
            await Assert.That(parent.Status).IsEqualTo(Status.Passed);
            await Assert.That(parent.Steps).Count()
                .IsEqualTo(expectedChildren.Length);
            await Assert.That(parent.Steps.Select(step => step.Name))
                .IsEquivalentTo(expectedChildren);
            await Assert.That(parent.Steps.Select(step => step.Status))
                .IsEquivalentTo(
                    Enumerable.Repeat(Status.Passed, expectedChildren.Length)
                );
            await Assert.That(parent.Steps.SelectMany(step => step.Steps))
                .IsEmpty();
        }

        await Assert.That(testResult.Steps.Sum(step => step.Steps.Count))
            .IsEqualTo(25);
    }

    [Test]
    public async Task ShouldRunTearDownAction()
    {
        var bodyCalled = false;
        var destination = await Run(
            () => this.TearDownAction(
                "teardown action",
                () => bodyCalled = true
            )
        );

        var container = await Assert.That(destination.TestContainers).HasSingleItem();
        var fixture = await Assert.That(container.Afters).HasSingleItem();
        await Assert.That(fixture.Name).IsEqualTo("teardown action");
        await Assert.That(fixture.Status).IsEqualTo(Status.Passed);
        await Assert.That(bodyCalled).IsTrue();
    }

    [Test]
    public async Task ShouldRunTearDownFunction()
    {
        var result = 0;
        var destination = await Run(async () =>
        {
            result = await this.TearDownFunction("teardown function", () => 42);
        });

        var container = await Assert.That(destination.TestContainers).HasSingleItem();
        var fixture = await Assert.That(container.Afters).HasSingleItem();
        await Assert.That(fixture.Name).IsEqualTo("teardown function");
        await Assert.That(fixture.Status).IsEqualTo(Status.Passed);
        await Assert.That(result).IsEqualTo(42);
    }

    static Task<InMemoryResultsDestination> Run(
        Func<Task> operation,
        OperationTarget target = OperationTarget.Test
    ) =>
        OperationTestApplication.RunAsync(operation, target);

    static async Task<string> CreateFile(
        byte[] content,
        string extension = ".txt"
    )
    {
        var path = Path.Combine(
            Path.GetTempPath(),
            $"allure-mtp-operation-{Guid.NewGuid():N}{extension}"
        );
        await File.WriteAllBytesAsync(path, content);
        return path;
    }

    static async Task AssertScreenDiff(
        InMemoryResultsDestination destination,
        string expected,
        string actual,
        string diff
    )
    {
        var source = destination.TestResults.Single().Attachments.Single().Source;
        using var descriptor = JsonDocument.Parse(
            destination.ByteAttachments[source]
        );

        await Assert.That(
            descriptor.RootElement.GetProperty("expected").GetString()
        ).IsEqualTo(expected);
        await Assert.That(
            descriptor.RootElement.GetProperty("actual").GetString()
        ).IsEqualTo(actual);
        await Assert.That(
            descriptor.RootElement.GetProperty("diff").GetString()
        ).IsEqualTo(diff);
    }
}
