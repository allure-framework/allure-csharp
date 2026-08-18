using Allure.TestingPlatform.Tests.Stubs;
using Allure.TestingPlatform.Sdk.Messages;
using Allure.Model;
using Allure.TestingPlatform.Sdk.Properties;
using System.Reflection;
using Allure.TestingPlatform.Sdk.Correlation;

namespace Allure.TestingPlatform.Tests.FixtureTests;

public abstract class FixturePropertyTestBase : DataConsumerTestsBase
{
    readonly CorrelationUid correlationUid = new("Bar");

    protected AllureScopeStartMessage StartScopeMessage { get; }
    protected AllureBeforeFixtureStartMessage StartFixtureMessage { get; }
    protected AllureFixtureUpdateMessage UpdateFixtureMessage { get; }
    protected AllureFixtureStopMessage StopFixtureMessage { get; }

    protected AllureScopeStopMessage StopScopeMessage { get; }

    public FixturePropertyTestBase()
    {
        this.StartScopeMessage = new AllureScopeStartMessage(correlationUid, new("1"));
        this.StartFixtureMessage = new AllureBeforeFixtureStartMessage(correlationUid, new("2"), new("1"), "Foo");
        this.UpdateFixtureMessage = new AllureFixtureUpdateMessage(correlationUid, new("2"));
        this.StopFixtureMessage = new AllureFixtureStopMessage(correlationUid, new("2"));
        this.StopScopeMessage = new AllureScopeStopMessage(correlationUid, new("1"));
    }

    protected async Task<FixtureResult> ArrangeAndAct(params IAllureProperty[] properties)
    {
        foreach (var property in properties)
        {
            this.PropertyListSelector.Add(property);
        }

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, this.StartScopeMessage, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, this.StartFixtureMessage, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, this.UpdateFixtureMessage, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, this.StopFixtureMessage, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, this.StopScopeMessage, CancellationToken.None);

        var container = await Assert.That(this.writer.TestContainers).HasSingleItem();
        return await Assert.That(container.Befores).HasSingleItem();
    }

    protected abstract List<IAllureProperty> PropertyListSelector { get; }

    [Test]
    public async Task ShouldUpdateFixtureName()
    {
        var fixture = await this.ArrangeAndAct(new AllureNameProperty<FixtureResult>("Updated name"));

        await Assert.That(fixture.Name).IsEqualTo("Updated name");
    }

    [Test]
    public async Task ShouldUpdateFixtureDescription()
    {
        var fixture = await this.ArrangeAndAct(new AllureDescriptionProperty<FixtureResult>("Lorem Ipsum"));

        await Assert.That(fixture.Description).IsEqualTo("Lorem Ipsum");
    }

    [Test]
    public async Task ShouldAppendDescriptionsIfAppendIsTrue()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureDescriptionProperty<FixtureResult>("Lorem Ipsum"),
            new AllureDescriptionProperty<FixtureResult>("Dolor Sit Amet") { Append = true }
        );

        await Assert.That(fixture.Description).IsEqualTo("Lorem Ipsum\n\nDolor Sit Amet");
    }

    [Test]
    public async Task ShouldUpdateFixtureDescriptionHtml()
    {
        var fixture = await this.ArrangeAndAct(new AllureDescriptionHtmlProperty<FixtureResult>("Lorem Ipsum"));

        await Assert.That(fixture.DescriptionHtml).IsEqualTo("Lorem Ipsum");
    }

    [Test]
    public async Task ShouldAppendHtmlDescriptionsIfAppendIsTrue()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureDescriptionHtmlProperty<FixtureResult>("Lorem Ipsum"),
            new AllureDescriptionHtmlProperty<FixtureResult>("Dolor Sit Amet") { Append = true }
        );

        await Assert.That(fixture.DescriptionHtml).IsEqualTo("Lorem IpsumDolor Sit Amet");
    }

    [Test]
    public async Task ShouldSetStartFromNumber()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureStartProperty<FixtureResult>(100)
        );

        await Assert.That(fixture.Start).IsEqualTo(100);
    }

    [Test]
    public async Task ShouldSetStartFromDateTime()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureStartProperty<FixtureResult>(DateTimeOffset.FromUnixTimeMilliseconds(100400))
        );

        await Assert.That(fixture.Start).IsEqualTo(100400);
    }

    [Test]
    public async Task ShouldSetStopFromNumber()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureStopProperty<FixtureResult>(100)
        );

        await Assert.That(fixture.Stop).IsEqualTo(100);
    }

    [Test]
    public async Task ShouldSetStopFromDateTime()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureStopProperty<FixtureResult>(DateTimeOffset.FromUnixTimeMilliseconds(100400))
        );

        await Assert.That(fixture.Stop).IsEqualTo(100400);
    }

    [Test]
    public async Task ShouldSetStopFromDurationNumber()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureStartProperty<FixtureResult>(1),
            new AllureDurationProperty<FixtureResult>(100)
        );

        await Assert.That(fixture.Stop).IsEqualTo(101);
    }

    [Test]
    public async Task ShouldSetStopFromDurationTimeSpan()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureStartProperty<FixtureResult>(1),
            new AllureDurationProperty<FixtureResult>(TimeSpan.FromMilliseconds(100))
        );

        await Assert.That(fixture.Stop).IsEqualTo(101);
    }

    [Test]
    public async Task ShouldSetStartFromDurationNumber()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureStopProperty<FixtureResult>(101),
            new AllureDurationProperty<FixtureResult>(100)
            {
                RelativeTo = AllureDurationAnchor.Stop
            }
        );

        await Assert.That(fixture.Start).IsEqualTo(1);
    }

    [Test]
    public async Task ShouldSetStartFromDurationTimeSpan()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureStopProperty<FixtureResult>(101),
            new AllureDurationProperty<FixtureResult>(TimeSpan.FromMilliseconds(100))
            {
                RelativeTo = AllureDurationAnchor.Stop
            }
        );

        await Assert.That(fixture.Start).IsEqualTo(1);
    }

    [Test]
    [Arguments(Status.Passed)]
    [Arguments(Status.Failed)]
    [Arguments(Status.Broken)]
    [Arguments(Status.Skipped)]
    public async Task ShouldSetStatus(Status expectedStatus)
    {
        var fixture = await this.ArrangeAndAct(
            new AllureStatusProperty<FixtureResult>(expectedStatus)
        );

        await Assert.That(fixture.Status).IsEqualTo(expectedStatus);
    }

    [Test]
    public async Task ShouldOverwriteStatusByDefault()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureStatusProperty<FixtureResult>(Status.Failed),
            new AllureStatusProperty<FixtureResult>(Status.Passed)
        );

        await Assert.That(testResult.Status).IsEqualTo(Status.Passed);
    }

    [Test]
    public async Task ShouldNotOverwriteAlreadySetStatusIfOptedOut()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureStatusProperty<FixtureResult>(Status.Failed),
            new AllureStatusProperty<FixtureResult>(Status.Passed)
            {
                OnlyIfUnset = true
            }
        );

        await Assert.That(testResult.Status).IsEqualTo(Status.Failed);
    }

    [Test]
    public async Task ShouldNotOverwriteDefaultStatusEvenIfOptedOutFromOverwrite()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureStatusProperty<FixtureResult>(Status.Passed)
            {
                OnlyIfUnset = true
            }
        );

        await Assert.That(testResult.Status).IsEqualTo(Status.Passed);
    }

    [Test]
    public async Task ShouldSetStatusDetails()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureStatusDetailsProperty<FixtureResult>(new()
            {
                Message = "Foo",
                Trace = "Bar",
                Known = true,
                Muted = true,
            })
        );

        await Assert.That(fixture.StatusDetails.Message).IsEqualTo("Foo");
        await Assert.That(fixture.StatusDetails.Trace).IsEqualTo("Bar");
        await Assert.That(fixture.StatusDetails.Known).IsTrue();
        await Assert.That(fixture.StatusDetails.Muted).IsTrue();
    }

    [Test]
    public async Task ShouldSetBrokenStatusAndDetailsFromError()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureExceptionProperty<FixtureResult>(new Exception("Foo"))
        );

        await Assert.That(fixture.Status).IsEqualTo(Status.Broken);
        await Assert.That(fixture.StatusDetails.Message).IsEqualTo("Foo");
        await Assert.That(fixture.StatusDetails.Trace).Contains("System.Exception");
    }

    static void TargetMethod(
        int p1,
        [AllureParameter(Name = "Foo")]
        string p2,
        [AllureParameter(Mode = ParameterMode.Masked)]
        int p3,
        [AllureParameter(Mode = ParameterMode.Hidden)]
        int p4,
        [AllureParameter(Ignore = true)]
        int p5
    ) {}

    [Test]
    public async Task ShouldAddMethodParameters()
    {
        var methodInfo =
            typeof(FixturePropertyTestBase)
                .GetMethod(
                    nameof(TargetMethod),
                    BindingFlags.Static | BindingFlags.NonPublic);

        var fixture = await this.ArrangeAndAct(
            new AllureTestMethodArgumentsProperty<FixtureResult>(
                methodInfo,
                [10, "foo", 20, 30, 40]
            )
        );

        await Assert.That(fixture.Parameters).Count().IsEqualTo(4);
        var parameter1 = fixture.Parameters[0];
        var parameter2 = fixture.Parameters[1];
        var parameter3 = fixture.Parameters[2];
        var parameter4 = fixture.Parameters[3];

        await Assert.That(parameter1.Name).IsEqualTo("p1");
        await Assert.That(parameter1.Value).IsEqualTo("10");
        await Assert.That(parameter1.Mode).IsNull();

        await Assert.That(parameter2.Name).IsEqualTo("Foo");
        await Assert.That(parameter2.Value).IsEqualTo("\"foo\"");
        await Assert.That(parameter2.Mode).IsNull();

        await Assert.That(parameter3.Name).IsEqualTo("p3");
        await Assert.That(parameter3.Value).IsEqualTo("20");
        await Assert.That(parameter3.Mode).IsEqualTo(ParameterMode.Masked);

        await Assert.That(parameter4.Name).IsEqualTo("p4");
        await Assert.That(parameter4.Value).IsEqualTo("30");
        await Assert.That(parameter4.Mode).IsEqualTo(ParameterMode.Hidden);
    }

    [Test]
    public async Task ShouldAddAllureParameters()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureParametersProperty<FixtureResult>(
                [
                    new(){ Name = "foo", Value = "1" },
                    new(){ Name = "bar", Value = "2" },
                ]
            )
        );

        await Assert.That(fixture.Parameters).Count().IsEqualTo(2);
        var parameter1 = fixture.Parameters[0];
        var parameter2 = fixture.Parameters[1];

        await Assert.That(parameter1.Name).IsEqualTo("foo");
        await Assert.That(parameter1.Value).IsEqualTo("1");

        await Assert.That(parameter2.Name).IsEqualTo("bar");
        await Assert.That(parameter2.Value).IsEqualTo("2");
    }

    [Test]
    public async Task ShouldAddAllureAttachment()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureAttachmentProperty<FixtureResult>(
                "Foo",
                new MemoryStream([1, 2, 3, 4])
            )
        );

        var attachment = await Assert.That(fixture.Attachments).HasSingleItem();
        await Assert.That(this.writer.ByteAttachments).ContainsKey(attachment.Source);
        await Assert.That(this.writer.ByteAttachments[attachment.Source]).IsEquivalentTo(
            new byte[]{ 1, 2, 3, 4 },
            TUnit.Assertions.Enums.CollectionOrdering.Matching
        );
        await Assert.That(attachment.Name).IsEqualTo("Foo");
        await Assert.That(attachment.Type).IsNull();
    }

    [Test]
    public async Task ShouldFillAttachmentContentType()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureAttachmentProperty<FixtureResult>(
                "Foo",
                new MemoryStream([1, 2, 3, 4])
            )
            {
                MediaType = "application/json"
            }
        );

        var attachment = await Assert.That(fixture.Attachments).HasSingleItem();
        await Assert.That(attachment.Type).IsEqualTo("application/json");
    }

    [Test]
    public async Task ShouldAppendExtensionToDestinationFileName()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureAttachmentProperty<FixtureResult>(
                "Foo",
                new MemoryStream([1, 2, 3, 4])
            )
            {
                FileExtension = ".txt"
            }
        );

        var attachment = await Assert.That(fixture.Attachments).HasSingleItem();
        await Assert.That(attachment.Source).EndsWith(".txt");
    }

    [Test]
    public async Task ShouldAddAllureAttachmentFile()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureAttachmentFileProperty<FixtureResult>(
                "Foo",
                "filepath"
            )
        );

        var attachment = await Assert.That(fixture.Attachments).HasSingleItem();
        await Assert.That(this.writer.FileAttachments).ContainsKey(attachment.Source);
        var relative = Path.GetRelativePath(
            Environment.CurrentDirectory,
            this.writer.FileAttachments[attachment.Source]
        );
        await Assert.That(relative).IsEqualTo("filepath");
        await Assert.That(attachment.Name).IsEqualTo("Foo");
        await Assert.That(attachment.Type).IsNull();
    }

    [Test]
    public async Task ShouldSetAllureAttachmentFileContentType()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureAttachmentFileProperty<FixtureResult>(
                "Foo",
                "filepath"
            )
            {
                MediaType = "application/json"
            }
        );

        var attachment = await Assert.That(fixture.Attachments).HasSingleItem();
        await Assert.That(attachment.Type).IsEqualTo("application/json");
    }

    [Test]
    public async Task ShouldAppendAttachmentFileExtensionToDestinationPath()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureAttachmentFileProperty<FixtureResult>(
                "Foo",
                "filepath"
            )
            {
                FileExtension = ".txt"
            }
        );

        var attachment = await Assert.That(fixture.Attachments).HasSingleItem();
        await Assert.That(attachment.Source).EndsWith(".txt");
    }

    [Test]
    public async Task ShouldUseFileExtensionByDefaultForAttachmentFiles()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureAttachmentFileProperty<FixtureResult>(
                "Foo",
                "filepath.txt"
            )
        );

        var attachment = await Assert.That(fixture.Attachments).HasSingleItem();
        await Assert.That(attachment.Source).EndsWith(".txt");
    }
}
