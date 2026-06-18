using Allure.TestingPlatform.Tests.Stubs;
using Allure.TestingPlatform.Messages;
using Allure.Net.Commons;
using Allure.TestingPlatform.Properties;
using Allure.Net.Commons.Attributes;
using System.Reflection;
using Allure.TestingPlatform.Sdk;

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
        return await Assert.That(container.befores).HasSingleItem();
    }

    protected abstract List<IAllureProperty> PropertyListSelector { get; }

    [Test]
    public async Task ShouldUpdateFixtureName()
    {
        var fixture = await this.ArrangeAndAct(new AllureNameProperty<FixtureResult>("Updated name"));

        await Assert.That(fixture.name).IsEqualTo("Updated name");
    }

    [Test]
    public async Task ShouldUpdateFixtureDescription()
    {
        var fixture = await this.ArrangeAndAct(new AllureDescriptionProperty<FixtureResult>("Lorem Ipsum"));

        await Assert.That(fixture.description).IsEqualTo("Lorem Ipsum");
    }

    [Test]
    public async Task ShouldAppendDescriptionsIfAppendIsTrue()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureDescriptionProperty<FixtureResult>("Lorem Ipsum"),
            new AllureDescriptionProperty<FixtureResult>("Dolor Sit Amet") { Append = true }
        );

        await Assert.That(fixture.description).IsEqualTo("Lorem Ipsum\n\nDolor Sit Amet");
    }

    [Test]
    public async Task ShouldUpdateFixtureDescriptionHtml()
    {
        var fixture = await this.ArrangeAndAct(new AllureDescriptionHtmlProperty<FixtureResult>("Lorem Ipsum"));

        await Assert.That(fixture.descriptionHtml).IsEqualTo("Lorem Ipsum");
    }

    [Test]
    public async Task ShouldAppendHtmlDescriptionsIfAppendIsTrue()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureDescriptionHtmlProperty<FixtureResult>("Lorem Ipsum"),
            new AllureDescriptionHtmlProperty<FixtureResult>("Dolor Sit Amet") { Append = true }
        );

        await Assert.That(fixture.descriptionHtml).IsEqualTo("Lorem IpsumDolor Sit Amet");
    }

    [Test]
    public async Task ShouldSetStartFromNumber()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureStartProperty<FixtureResult>(100)
        );

        await Assert.That(fixture.start).IsEqualTo(100);
    }

    [Test]
    public async Task ShouldSetStartFromDateTime()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureStartProperty<FixtureResult>(DateTimeOffset.FromUnixTimeMilliseconds(100400))
        );

        await Assert.That(fixture.start).IsEqualTo(100400);
    }

    [Test]
    public async Task ShouldSetStopFromNumber()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureStopProperty<FixtureResult>(100)
        );

        await Assert.That(fixture.stop).IsEqualTo(100);
    }

    [Test]
    public async Task ShouldSetStopFromDateTime()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureStopProperty<FixtureResult>(DateTimeOffset.FromUnixTimeMilliseconds(100400))
        );

        await Assert.That(fixture.stop).IsEqualTo(100400);
    }

    [Test]
    public async Task ShouldSetStopFromDurationNumber()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureStartProperty<FixtureResult>(1),
            new AllureDurationProperty<FixtureResult>(100)
        );

        await Assert.That(fixture.stop).IsEqualTo(101);
    }

    [Test]
    public async Task ShouldSetStopFromDurationTimeSpan()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureStartProperty<FixtureResult>(1),
            new AllureDurationProperty<FixtureResult>(TimeSpan.FromMilliseconds(100))
        );

        await Assert.That(fixture.stop).IsEqualTo(101);
    }

    [Test]
    public async Task ShouldSetStartFromDurationNumber()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureStopProperty<FixtureResult>(101),
            new AllureDurationProperty<FixtureResult>(100)
            {
                RelativeTo = DurationBase.Stop
            }
        );

        await Assert.That(fixture.start).IsEqualTo(1);
    }

    [Test]
    public async Task ShouldSetStartFromDurationTimeSpan()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureStopProperty<FixtureResult>(101),
            new AllureDurationProperty<FixtureResult>(TimeSpan.FromMilliseconds(100))
            {
                RelativeTo = DurationBase.Stop
            }
        );

        await Assert.That(fixture.start).IsEqualTo(1);
    }

    [Test]
    [Arguments(Status.passed)]
    [Arguments(Status.failed)]
    [Arguments(Status.broken)]
    [Arguments(Status.skipped)]
    public async Task ShouldSetStatus(Status expectedStatus)
    {
        var fixture = await this.ArrangeAndAct(
            new AllureStatusProperty<FixtureResult>(expectedStatus)
        );

        await Assert.That(fixture.status).IsEqualTo(expectedStatus);
    }

    [Test]
    public async Task ShouldSetStatusDetails()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureStatusDetailsProperty<FixtureResult>(new()
            {
                message = "Foo",
                trace = "Bar",
                known = true,
                muted = true,
            })
        );

        await Assert.That(fixture.statusDetails.message).IsEqualTo("Foo");
        await Assert.That(fixture.statusDetails.trace).IsEqualTo("Bar");
        await Assert.That(fixture.statusDetails.known).IsTrue();
        await Assert.That(fixture.statusDetails.muted).IsTrue();
    }

    [Test]
    public async Task ShouldSetBrokenStatusAndDetailsFromError()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureExceptionProperty<FixtureResult>(new Exception("Foo"))
        );

        await Assert.That(fixture.status).IsEqualTo(Status.broken);
        await Assert.That(fixture.statusDetails.message).IsEqualTo("Foo");
        await Assert.That(fixture.statusDetails.trace).Contains("System.Exception");
    }

    [Test]
    public async Task ShouldSetFailedStatusAndDetailsFromError()
    {
        this.config.FailExceptions.Add("System.Exception");

        var fixture = await this.ArrangeAndAct(
            new AllureExceptionProperty<FixtureResult>(new Exception("Foo"))
        );

        await Assert.That(fixture.status).IsEqualTo(Status.failed);
        await Assert.That(fixture.statusDetails.message).IsEqualTo("Foo");
        await Assert.That(fixture.statusDetails.trace).Contains("System.Exception");
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

        await Assert.That(fixture.parameters).Count().IsEqualTo(4);
        var parameter1 = fixture.parameters[0];
        var parameter2 = fixture.parameters[1];
        var parameter3 = fixture.parameters[2];
        var parameter4 = fixture.parameters[3];

        await Assert.That(parameter1.name).IsEqualTo("p1");
        await Assert.That(parameter1.value).IsEqualTo("10");
        await Assert.That(parameter1.mode).IsNull();

        await Assert.That(parameter2.name).IsEqualTo("Foo");
        await Assert.That(parameter2.value).IsEqualTo("\"foo\"");
        await Assert.That(parameter2.mode).IsNull();

        await Assert.That(parameter3.name).IsEqualTo("p3");
        await Assert.That(parameter3.value).IsEqualTo("20");
        await Assert.That(parameter3.mode).IsEqualTo(ParameterMode.Masked);

        await Assert.That(parameter4.name).IsEqualTo("p4");
        await Assert.That(parameter4.value).IsEqualTo("30");
        await Assert.That(parameter4.mode).IsEqualTo(ParameterMode.Hidden);
    }

    [Test]
    public async Task ShouldAddAllureParameters()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureParametersProperty<FixtureResult>(
                [
                    new(){ name = "foo", value = "1" },
                    new(){ name = "bar", value = "2" },
                ]
            )
        );

        await Assert.That(fixture.parameters).Count().IsEqualTo(2);
        var parameter1 = fixture.parameters[0];
        var parameter2 = fixture.parameters[1];

        await Assert.That(parameter1.name).IsEqualTo("foo");
        await Assert.That(parameter1.value).IsEqualTo("1");

        await Assert.That(parameter2.name).IsEqualTo("bar");
        await Assert.That(parameter2.value).IsEqualTo("2");
    }

    [Test]
    public async Task ShouldAddAllureAttachment()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureAttachmentProperty<FixtureResult>(
                "Foo",
                [1, 2, 3, 4]
            )
        );

        var attachment = await Assert.That(fixture.attachments).HasSingleItem();
        await Assert.That(this.writer.ByteAttachments).ContainsKey(attachment.source);
        await Assert.That(this.writer.ByteAttachments[attachment.source]).IsEquivalentTo(
            new byte[]{ 1, 2, 3, 4 },
            TUnit.Assertions.Enums.CollectionOrdering.Matching
        );
        await Assert.That(attachment.name).IsEqualTo("Foo");
        await Assert.That(attachment.type).IsNull();
    }

    [Test]
    public async Task ShouldFillAttachmentContentType()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureAttachmentProperty<FixtureResult>(
                "Foo",
                [1, 2, 3, 4]
            )
            {
                ContentType = "application/json"
            }
        );

        var attachment = await Assert.That(fixture.attachments).HasSingleItem();
        await Assert.That(attachment.type).IsEqualTo("application/json");
    }

    [Test]
    public async Task ShouldAppendExtensionToDestinationFileName()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureAttachmentProperty<FixtureResult>(
                "Foo",
                [1, 2, 3, 4]
            )
            {
                FileExtension = ".txt"
            }
        );

        var attachment = await Assert.That(fixture.attachments).HasSingleItem();
        await Assert.That(attachment.source).EndsWith(".txt");
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

        var attachment = await Assert.That(fixture.attachments).HasSingleItem();
        await Assert.That(this.writer.FileAttachments).ContainsKey(attachment.source);
        var relative = Path.GetRelativePath(
            Environment.CurrentDirectory,
            this.writer.FileAttachments[attachment.source]
        );
        await Assert.That(relative).IsEqualTo("filepath");
        await Assert.That(attachment.name).IsEqualTo("Foo");
        await Assert.That(attachment.type).IsNull();
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
                ContentType = "application/json"
            }
        );

        var attachment = await Assert.That(fixture.attachments).HasSingleItem();
        await Assert.That(attachment.type).IsEqualTo("application/json");
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

        var attachment = await Assert.That(fixture.attachments).HasSingleItem();
        await Assert.That(attachment.source).EndsWith(".txt");
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

        var attachment = await Assert.That(fixture.attachments).HasSingleItem();
        await Assert.That(attachment.source).EndsWith(".txt");
    }
}
