using Microsoft.Testing.Platform.TestHost;
using Allure.TestingPlatform.Tests.Stubs;
using Allure.TestingPlatform.Sdk.Messages;
using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Properties;
using Allure.Net.Commons.Attributes;
using System.Reflection;
using Microsoft.Testing.Platform.Extensions.Messages;
using Allure.TestingPlatform.Tests.Comparers;
using Allure.TestingPlatform.Sdk.Runtime.Correlation;

namespace Allure.TestingPlatform.Tests;

using AllureTestResult = Net.Commons.TestResult;

public class TestPropertyTests : DataConsumerTestsBase
{
    readonly SessionUid sessionUid = new("Bar");
    readonly CorrelationUid correlationUid = new("Bar");

    async Task<AllureTestResult> ArrangeAndAct(params IAllureProperty<AllureTestResult>[] properties)
    {
        var testNodeInProgress = new TestNodeUpdateMessage(
            sessionUid,
            new()
            {
                Uid = "1",
                DisplayName = "Foo",
                Properties = new(new InProgressTestNodeStateProperty())
            }
        );

        var updateTest = new AllureTestUpdateMessage(correlationUid, new("1"))
        {
            Properties = [..properties],
        };

        var testNodePassed = new TestNodeUpdateMessage(
            sessionUid,
            new()
            {
                Uid = "1",
                DisplayName = "Foo",
                Properties = new(new PassedTestNodeStateProperty())
            }
        );

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testNodeInProgress, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, updateTest, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testNodePassed, CancellationToken.None);

        return await Assert.That(this.writer.TestResults).HasSingleItem();
    }

    [Test]
    public async Task ShouldUpdateTestName()
    {
        var testResult = await this.ArrangeAndAct(new AllureNameProperty<AllureTestResult>("Updated name"));

        await Assert.That(testResult.name).IsEqualTo("Updated name");
    }

    [Test]
    public async Task ShouldUpdateTestDescription()
    {
        var testResult = await this.ArrangeAndAct(new AllureDescriptionProperty<AllureTestResult>("Lorem Ipsum"));

        await Assert.That(testResult.description).IsEqualTo("Lorem Ipsum");
    }

    [Test]
    public async Task ShouldAppendDescriptionsIfAppendIsTrue()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureDescriptionProperty<AllureTestResult>("Lorem Ipsum"),
            new AllureDescriptionProperty<AllureTestResult>("Dolor Sit Amet") { Append = true }
        );

        await Assert.That(testResult.description).IsEqualTo("Lorem Ipsum\n\nDolor Sit Amet");
    }

    [Test]
    public async Task ShouldUpdateTestDescriptionHtml()
    {
        var testResult = await this.ArrangeAndAct(new AllureDescriptionHtmlProperty<AllureTestResult>("Lorem Ipsum"));

        await Assert.That(testResult.descriptionHtml).IsEqualTo("Lorem Ipsum");
    }

    [Test]
    public async Task ShouldAppendHtmlDescriptionsIfAppendIsTrue()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureDescriptionHtmlProperty<AllureTestResult>("Lorem Ipsum"),
            new AllureDescriptionHtmlProperty<AllureTestResult>("Dolor Sit Amet") { Append = true }
        );

        await Assert.That(testResult.descriptionHtml).IsEqualTo("Lorem IpsumDolor Sit Amet");
    }

    [Test]
    public async Task ShouldSetStartFromNumber()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureStartProperty<AllureTestResult>(100)
        );

        await Assert.That(testResult.start).IsEqualTo(100);
    }

    [Test]
    public async Task ShouldSetStartFromDateTime()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureStartProperty<AllureTestResult>(DateTimeOffset.FromUnixTimeMilliseconds(100400))
        );

        await Assert.That(testResult.start).IsEqualTo(100400);
    }

    [Test]
    public async Task ShouldSetStopFromNumber()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureStopProperty<AllureTestResult>(100)
        );

        await Assert.That(testResult.stop).IsEqualTo(100);
    }

    [Test]
    public async Task ShouldSetStopFromDateTime()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureStopProperty<AllureTestResult>(DateTimeOffset.FromUnixTimeMilliseconds(100400))
        );

        await Assert.That(testResult.stop).IsEqualTo(100400);
    }

    [Test]
    public async Task ShouldSetStopFromDurationNumber()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureStartProperty<AllureTestResult>(1),
            new AllureDurationProperty<AllureTestResult>(100)
        );

        await Assert.That(testResult.stop).IsEqualTo(101);
    }

    [Test]
    public async Task ShouldSetStopFromDurationTimeSpan()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureStartProperty<AllureTestResult>(1),
            new AllureDurationProperty<AllureTestResult>(TimeSpan.FromMilliseconds(100))
        );

        await Assert.That(testResult.stop).IsEqualTo(101);
    }

    [Test]
    public async Task ShouldSetStartFromDurationNumber()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureStopProperty<AllureTestResult>(101),
            new AllureDurationProperty<AllureTestResult>(100)
            {
                RelativeTo = DurationBase.Stop
            }
        );

        await Assert.That(testResult.start).IsEqualTo(1);
    }

    [Test]
    public async Task ShouldSetStartFromDurationTimeSpan()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureStopProperty<AllureTestResult>(101),
            new AllureDurationProperty<AllureTestResult>(TimeSpan.FromMilliseconds(100))
            {
                RelativeTo = DurationBase.Stop
            }
        );

        await Assert.That(testResult.start).IsEqualTo(1);
    }

    [Test]
    [Arguments(Status.passed)]
    [Arguments(Status.failed)]
    [Arguments(Status.broken)]
    [Arguments(Status.skipped)]
    public async Task ShouldSetStatus(Status expectedStatus)
    {
        var testResult = await this.ArrangeAndAct(
            new AllureStatusProperty<AllureTestResult>(expectedStatus)
        );

        await Assert.That(testResult.status).IsEqualTo(expectedStatus);
    }

    [Test]
    public async Task ShouldOverwriteStatusByDefault()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureStatusProperty<AllureTestResult>(Status.failed),
            new AllureStatusProperty<AllureTestResult>(Status.passed)
        );

        await Assert.That(testResult.status).IsEqualTo(Status.passed);
    }

    [Test]
    public async Task ShouldNotOverwriteAlreadySetStatusIfOptedOut()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureStatusProperty<AllureTestResult>(Status.failed),
            new AllureStatusProperty<AllureTestResult>(Status.passed)
            {
                OverwriteDefaultOnly = true
            }
        );

        await Assert.That(testResult.status).IsEqualTo(Status.failed);
    }

    [Test]
    public async Task ShouldNotOverwriteDefaultStatusEvenIfOptedOutFromOverwrite()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureStatusProperty<AllureTestResult>(Status.passed)
            {
                OverwriteDefaultOnly = true
            }
        );

        await Assert.That(testResult.status).IsEqualTo(Status.passed);
    }

    [Test]
    public async Task ShouldSetStatusDetails()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureStatusDetailsProperty<AllureTestResult>(new()
            {
                message = "Foo",
                trace = "Bar",
                known = true,
                muted = true,
            })
        );

        await Assert.That(testResult.statusDetails.message).IsEqualTo("Foo");
        await Assert.That(testResult.statusDetails.trace).IsEqualTo("Bar");
        await Assert.That(testResult.statusDetails.known).IsTrue();
        await Assert.That(testResult.statusDetails.muted).IsTrue();
    }

    [Test]
    public async Task ShouldSetBrokenStatusAndDetailsFromError()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureExceptionProperty<AllureTestResult>(new Exception("Foo"))
        );

        await Assert.That(testResult.status).IsEqualTo(Status.broken);
        await Assert.That(testResult.statusDetails.message).IsEqualTo("Foo");
        await Assert.That(testResult.statusDetails.trace).Contains("System.Exception");
    }

    [Test]
    public async Task ShouldSetFailedStatusAndDetailsFromError()
    {
        this.config.FailExceptions = ["System.Exception"];

        var testResult = await this.ArrangeAndAct(
            new AllureExceptionProperty<AllureTestResult>(new Exception("Foo"))
        );

        await Assert.That(testResult.status).IsEqualTo(Status.failed);
        await Assert.That(testResult.statusDetails.message).IsEqualTo("Foo");
        await Assert.That(testResult.statusDetails.trace).Contains("System.Exception");
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
            typeof(TestPropertyTests)
                .GetMethod(
                    nameof(TargetMethod),
                    BindingFlags.Static | BindingFlags.NonPublic);

        var testResult = await this.ArrangeAndAct(
            new AllureTestMethodArgumentsProperty<AllureTestResult>(
                methodInfo,
                [10, "foo", 20, 30, 40]
            )
        );

        await Assert.That(testResult.parameters).Count().IsEqualTo(4);
        var parameter1 = testResult.parameters[0];
        var parameter2 = testResult.parameters[1];
        var parameter3 = testResult.parameters[2];
        var parameter4 = testResult.parameters[3];

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
        var testResult = await this.ArrangeAndAct(
            new AllureParametersProperty<AllureTestResult>(
                [
                    new(){ name = "foo", value = "1" },
                    new(){ name = "bar", value = "2" },
                ]
            )
        );

        await Assert.That(testResult.parameters).Count().IsEqualTo(2);
        var parameter1 = testResult.parameters[0];
        var parameter2 = testResult.parameters[1];

        await Assert.That(parameter1.name).IsEqualTo("foo");
        await Assert.That(parameter1.value).IsEqualTo("1");

        await Assert.That(parameter2.name).IsEqualTo("bar");
        await Assert.That(parameter2.value).IsEqualTo("2");
    }

    [Test]
    public async Task ShouldAddAllureAttachment()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureAttachmentProperty<AllureTestResult>(
                "Foo",
                [1, 2, 3, 4]
            )
        );

        var attachment = await Assert.That(testResult.attachments).HasSingleItem();
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
        var testResult = await this.ArrangeAndAct(
            new AllureAttachmentProperty<AllureTestResult>(
                "Foo",
                [1, 2, 3, 4]
            )
            {
                ContentType = "application/json"
            }
        );

        var attachment = await Assert.That(testResult.attachments).HasSingleItem();
        await Assert.That(attachment.type).IsEqualTo("application/json");
    }

    [Test]
    public async Task ShouldAppendExtensionToDestinationFileName()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureAttachmentProperty<AllureTestResult>(
                "Foo",
                [1, 2, 3, 4]
            )
            {
                FileExtension = ".txt"
            }
        );

        var attachment = await Assert.That(testResult.attachments).HasSingleItem();
        await Assert.That(attachment.source).EndsWith(".txt");
    }

    [Test]
    public async Task ShouldAddAllureAttachmentFile()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureAttachmentFileProperty<AllureTestResult>(
                "Foo",
                "filepath"
            )
        );

        var attachment = await Assert.That(testResult.attachments).HasSingleItem();
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
        var testResult = await this.ArrangeAndAct(
            new AllureAttachmentFileProperty<AllureTestResult>(
                "Foo",
                "filepath"
            )
            {
                ContentType = "application/json"
            }
        );

        var attachment = await Assert.That(testResult.attachments).HasSingleItem();
        await Assert.That(attachment.type).IsEqualTo("application/json");
    }

    [Test]
    public async Task ShouldAppendAttachmentFileExtensionToDestinationPath()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureAttachmentFileProperty<AllureTestResult>(
                "Foo",
                "filepath"
            )
            {
                FileExtension = ".txt"
            }
        );

        var attachment = await Assert.That(testResult.attachments).HasSingleItem();
        await Assert.That(attachment.source).EndsWith(".txt");
    }

    [Test]
    public async Task ShouldUseFileExtensionByDefaultForAttachmentFiles()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureAttachmentFileProperty<AllureTestResult>(
                "Foo",
                "filepath.txt"
            )
        );

        var attachment = await Assert.That(testResult.attachments).HasSingleItem();
        await Assert.That(attachment.source).EndsWith(".txt");
    }

    [Test]
    public async Task ShouldAddLabels()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureLabelsProperty([
                new(){ name = "foo", value = "bar" },
                new(){ name = "baz", value = "qux" },
            ])
        );

        await Assert.That(testResult.labels)
            .Contains(l => l.name == "foo" && l.value == "bar")
            .And.Contains(l => l.name == "baz" && l.value == "qux");
    }

    [Test]
    public async Task ShouldAddLinks()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureLinksProperty([
                new(){ name = "foo", url = "bar" },
                new(){ name = "baz", url = "qux", type = "qut" },
            ])
        );

        await Assert.That(testResult.links)
            .Contains(l => l.name == "foo" && l.url == "bar" && l.type is null)
            .And.Contains(l => l.name == "baz" && l.url == "qux" && l.type == "qut");
    }

    [Test]
    public async Task ShouldAddDefaultSuites()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureDefaultSuitesProperty("foo", "bar", "baz")
        );

        var parentSuite = await Assert.That(testResult.labels).HasSingleItem(l => l.name == "parentSuite");
        var suite = await Assert.That(testResult.labels).HasSingleItem(l => l.name == "suite");
        var subSuite = await Assert.That(testResult.labels).HasSingleItem(l => l.name == "subSuite");
        await Assert.That(parentSuite.value).IsEqualTo("foo");
        await Assert.That(suite.value).IsEqualTo("bar");
        await Assert.That(subSuite.value).IsEqualTo("baz");
    }

    [Test]
    public async Task ShouldAddDefaultSuitesFromType()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureDefaultSuitesProperty(typeof(TargetClass))
        );

        var parentSuite = await Assert.That(testResult.labels).HasSingleItem(l => l.name == "parentSuite");
        var suite = await Assert.That(testResult.labels).HasSingleItem(l => l.name == "suite");
        var subSuite = await Assert.That(testResult.labels).HasSingleItem(l => l.name == "subSuite");
        await Assert.That(parentSuite.value).IsEqualTo("Allure.TestingPlatform.Tests");
        await Assert.That(suite.value).IsEqualTo("Allure.TestingPlatform.Tests");
        await Assert.That(subSuite.value).IsEqualTo("TargetClass");
    }

    [Test]
    public async Task ShouldRespectAllureNameAttributeWhenAddingDefaultSubSuitesFromType()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureDefaultSuitesProperty(typeof(TargetClassNamed))
        );

        await Assert.That(testResult.labels)
            .Contains(l => l.name == "subSuite" && l.value == "Foo");
    }

    [Test]
    public async Task ShouldNotAddDefaultSuitesIfParentSuitePresent()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureLabelsProperty([new(){ name = "parentSuite", value = "qux" }]),
            new AllureDefaultSuitesProperty("foo", "bar", "baz")
        );

        var parentSuite = await Assert.That(testResult.labels).HasSingleItem(l => l.name == "parentSuite");
        await Assert.That(parentSuite.value).IsEqualTo("qux");
        await Assert.That(testResult.labels)
            .DoesNotContain(l => l.name == "suite")
            .And.DoesNotContain(l => l.name == "subSuite");
    }

    [Test]
    public async Task ShouldNotAddDefaultSuitesIfSuitePresent()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureLabelsProperty([new(){ name = "suite", value = "qux" }]),
            new AllureDefaultSuitesProperty("foo", "bar", "baz")
        );

        var suite = await Assert.That(testResult.labels).HasSingleItem(l => l.name == "suite");
        await Assert.That(suite.value).IsEqualTo("qux");
        await Assert.That(testResult.labels)
            .DoesNotContain(l => l.name == "parentSuite")
            .And.DoesNotContain(l => l.name == "subSuite");
    }

    [Test]
    public async Task ShouldNotAddDefaultSuitesIfSubSuitePresent()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureLabelsProperty([new(){ name = "subSuite", value = "qux" }]),
            new AllureDefaultSuitesProperty("foo", "bar", "baz")
        );

        var subSuite = await Assert.That(testResult.labels).HasSingleItem(l => l.name == "subSuite");
        await Assert.That(subSuite.value).IsEqualTo("qux");
        await Assert.That(testResult.labels)
            .DoesNotContain(l => l.name == "parentSuite")
            .And.DoesNotContain(l => l.name == "suite");
    }

    [Test]
    public async Task ShouldApplyTestMethodMetadata()
    {
        var method =
            typeof(TargetClass)
                .GetMethod(
                    nameof(TargetClass.TargetMethod),
                    BindingFlags.Static | BindingFlags.Public);

        var testResult = await this.ArrangeAndAct(
            new AllureTestMethodProperty(method)
            {
                Arguments = [1, 2, "foo", 3, 4, 5]
            }
        );

        await Assert.That(testResult.fullName).IsEqualTo(
            "Allure.TestingPlatform.Tests:Allure.TestingPlatform.Tests.TargetClass.TargetMethod(System.Int32,System.Int32,System.String,System.Int32,System.Int32,System.Int32)"
        );
        await Assert.That(testResult.titlePath).IsEquivalentTo([
            "Allure.TestingPlatform.Tests",
            "Allure",
            "TestingPlatform",
            "Tests",
            "TargetClass",
            "TargetMethod(System.Int32,System.Int32,System.String,System.Int32,System.Int32,System.Int32)",
        ]);
        await Assert.That(testResult.labels)
            .Contains(l => l.name == "testClass" && l.value == "TargetClass")
            .And.Contains(l => l.name == "testMethod" && l.value == "TargetMethod")
            .And.Contains(l => l.name == "package" && l.value == "Allure.TestingPlatform.Tests.TargetClass");
        await Assert.That(testResult.parameters).IsEquivalentTo(
            [
                new Parameter() { name = "p1", value = "1" },
                new Parameter() { name = "Foo", value = "2" },
                new Parameter() { name = "p3", value = "\"foo\"", mode = ParameterMode.Masked },
                new Parameter() { name = "p4", value = "3", mode = ParameterMode.Hidden },
                new Parameter() { name = "p5", value = "4", excluded = true },
            ],
            ParameterComparer.Instance,
            TUnit.Assertions.Enums.CollectionOrdering.Matching
        );

        await Assert.That(testResult.labels)
            .Contains(l => l.name == "epic" && l.value == "foo")
            .And.Contains(l => l.name == "feature" && l.value == "bar");
    }

    [Test]
    public async Task ShouldRespectAllureNameAttributeWhenApplyingTestMethodMetadata()
    {
        var method =
            typeof(TargetClassNamed)
                .GetMethod(
                    nameof(TargetClassNamed.TargetMethodNamedWithParameters),
                    BindingFlags.Static | BindingFlags.Public);

        var testResult = await this.ArrangeAndAct(
            new AllureTestMethodProperty(method)
        );

        await Assert.That(testResult.titlePath).IsEquivalentTo([
            "Allure.TestingPlatform.Tests",
            "Allure",
            "TestingPlatform",
            "Tests",
            "Foo",
            "Bar",
        ]);
    }

    [Test]
    public async Task ShouldNotAddMethodNoteToTitlePathForNonParameterizedMethods()
    {
        var method =
            typeof(TargetClassNamed)
                .GetMethod(
                    nameof(TargetClassNamed.TargetMethodNamedNoParameters),
                    BindingFlags.Static | BindingFlags.Public);

        var testResult = await this.ArrangeAndAct(
            new AllureTestMethodProperty(method)
        );

        await Assert.That(testResult.titlePath).IsEquivalentTo([
            "Allure.TestingPlatform.Tests",
            "Allure",
            "TestingPlatform",
            "Tests",
            "Foo",
        ]);
    }

    [Test]
    public async Task ShouldIgnoreFullNameIfTestMethodMetadataPropertyInstructsThat()
    {
        var method =
            typeof(TargetClass)
                .GetMethod(
                    nameof(TargetClass.TargetMethod),
                    BindingFlags.Static | BindingFlags.Public);

        var testResult = await this.ArrangeAndAct(
            new AllureTestMethodProperty(method)
            {
                Arguments = [1, 2, "foo", 3, 4, 5],
                UpdateTargets = TestMethodUpdateTarget.All & (~TestMethodUpdateTarget.FullName),
            }
        );

        await Assert.That(testResult.fullName).IsEqualTo("1");
    }

    [Test]
    public async Task ShouldIgnoreTitlePathIfTestMethodMetadataPropertyInstructsThat()
    {
        var method =
            typeof(TargetClass)
                .GetMethod(
                    nameof(TargetClass.TargetMethod),
                    BindingFlags.Static | BindingFlags.Public);

        var testResult = await this.ArrangeAndAct(
            new AllureTestMethodProperty(method)
            {
                Arguments = [1, 2, "foo", 3, 4, 5],
                UpdateTargets = TestMethodUpdateTarget.All & (~TestMethodUpdateTarget.TitlePath),
            }
        );

        await Assert.That(testResult.titlePath).IsEmpty();
    }

    [Test]
    public async Task ShouldIgnoreLabelsIfTestMethodMetadataPropertyInstructsThat()
    {
        var method =
            typeof(TargetClass)
                .GetMethod(
                    nameof(TargetClass.TargetMethod),
                    BindingFlags.Static | BindingFlags.Public);

        var testResult = await this.ArrangeAndAct(
            new AllureTestMethodProperty(method)
            {
                Arguments = [1, 2, "foo", 3, 4, 5],
                UpdateTargets = TestMethodUpdateTarget.All & (~TestMethodUpdateTarget.Labels),
            }
        );

        await Assert.That(testResult.labels)
            .DoesNotContain(l => l.name == "testClass")
            .And.DoesNotContain(l => l.name == "testMethod")
            .And.DoesNotContain(l => l.name == "package");
    }

    [Test]
    public async Task ShouldIgnoreParametersIfTestMethodMetadataPropertyInstructsThat()
    {
        var method =
            typeof(TargetClass)
                .GetMethod(
                    nameof(TargetClass.TargetMethod),
                    BindingFlags.Static | BindingFlags.Public);

        var testResult = await this.ArrangeAndAct(
            new AllureTestMethodProperty(method)
            {
                Arguments = [1, 2, "foo", 3, 4, 5],
                UpdateTargets = TestMethodUpdateTarget.All & (~TestMethodUpdateTarget.Parameters),
            }
        );

        await Assert.That(testResult.parameters).IsEmpty();
    }

    [Test]
    public async Task ShouldIgnoreApiAttributesIfTestMethodMetadataPropertyInstructsThat()
    {
        var method =
            typeof(TargetClass)
                .GetMethod(
                    nameof(TargetClass.TargetMethod),
                    BindingFlags.Static | BindingFlags.Public);

        var testResult = await this.ArrangeAndAct(
            new AllureTestMethodProperty(method)
            {
                Arguments = [1, 2, "foo", 3, 4, 5],
                UpdateTargets = TestMethodUpdateTarget.All & (~TestMethodUpdateTarget.ApiAttributes),
            }
        );

        await Assert.That(testResult.labels)
            .DoesNotContain(l => l.name == "epic")
            .And.DoesNotContain(l => l.name == "feature");
    }

    [Test]
    public async Task ShouldApplyTestMethodArguments()
    {
        var method =
            typeof(TargetClass)
                .GetMethod(
                    nameof(TargetClass.TargetMethod),
                    BindingFlags.Static | BindingFlags.Public);

        var testResult = await this.ArrangeAndAct(
            new AllureTestMethodArgumentsProperty<AllureTestResult>(
                method,
                [1, 2, "foo", 3, 4, 5])
        );

        await Assert.That(testResult.parameters).IsEquivalentTo(
            [
                new Parameter() { name = "p1", value = "1" },
                new Parameter() { name = "Foo", value = "2" },
                new Parameter() { name = "p3", value = "\"foo\"", mode = ParameterMode.Masked },
                new Parameter() { name = "p4", value = "3", mode = ParameterMode.Hidden },
                new Parameter() { name = "p5", value = "4", excluded = true },
            ],
            ParameterComparer.Instance,
            TUnit.Assertions.Enums.CollectionOrdering.Matching
        );
    }

    [Test]
    public async Task ShouldApplyFullName()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureFullNameProperty("foo")
        );

        await Assert.That(testResult.fullName).IsEqualTo("foo");
    }

    [Test]
    public async Task ShouldApplyTitlePath()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureTitlePathProperty(["foo", "bar"])
        );

        await Assert.That(testResult.titlePath).IsEquivalentTo(
            ["foo", "bar"],
            TUnit.Assertions.Enums.CollectionOrdering.Matching
        );
    }
}

[AllureEpic("foo")]
class TargetClass
{
    [AllureFeature("bar")]
    public static void TargetMethod(
        int p1,
        [AllureParameter(Name = "Foo")]
        int p2,
        [AllureParameter(Mode = ParameterMode.Masked)]
        string p3,
        [AllureParameter(Mode = ParameterMode.Hidden)]
        int p4,
        [AllureParameter(Excluded = true)]
        int p5,
        [AllureParameter(Ignore = true)]
        int p6
    ) { }
}

[AllureName("Foo")]
class TargetClassNamed
{
    [AllureName("Bar")]
    public static void TargetMethodNamedNoParameters() { }

    [AllureName("Bar")]
    public static void TargetMethodNamedWithParameters(int p) { }
}
