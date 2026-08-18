using Microsoft.Testing.Platform.TestHost;
using Allure.TestingPlatform.Tests.Stubs;
using Allure.TestingPlatform.Sdk.Messages;
using Allure.Model;
using Allure.TestingPlatform.Sdk.Properties;
using System.Reflection;
using Microsoft.Testing.Platform.Extensions.Messages;
using Allure.TestingPlatform.Tests.Comparers;
using Allure.TestingPlatform.Sdk.Correlation;

namespace Allure.TestingPlatform.Tests;

using AllureTestResult = Model.TestResult;

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

        await Assert.That(testResult.Name).IsEqualTo("Updated name");
    }

    [Test]
    public async Task ShouldUpdateTestDescription()
    {
        var testResult = await this.ArrangeAndAct(new AllureDescriptionProperty<AllureTestResult>("Lorem Ipsum"));

        await Assert.That(testResult.Description).IsEqualTo("Lorem Ipsum");
    }

    [Test]
    public async Task ShouldAppendDescriptionsIfAppendIsTrue()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureDescriptionProperty<AllureTestResult>("Lorem Ipsum"),
            new AllureDescriptionProperty<AllureTestResult>("Dolor Sit Amet") { Append = true }
        );

        await Assert.That(testResult.Description).IsEqualTo("Lorem Ipsum\n\nDolor Sit Amet");
    }

    [Test]
    public async Task ShouldUpdateTestDescriptionHtml()
    {
        var testResult = await this.ArrangeAndAct(new AllureDescriptionHtmlProperty<AllureTestResult>("Lorem Ipsum"));

        await Assert.That(testResult.DescriptionHtml).IsEqualTo("Lorem Ipsum");
    }

    [Test]
    public async Task ShouldAppendHtmlDescriptionsIfAppendIsTrue()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureDescriptionHtmlProperty<AllureTestResult>("Lorem Ipsum"),
            new AllureDescriptionHtmlProperty<AllureTestResult>("Dolor Sit Amet") { Append = true }
        );

        await Assert.That(testResult.DescriptionHtml).IsEqualTo("Lorem IpsumDolor Sit Amet");
    }

    [Test]
    public async Task ShouldSetStartFromNumber()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureStartProperty<AllureTestResult>(100)
        );

        await Assert.That(testResult.Start).IsEqualTo(100);
    }

    [Test]
    public async Task ShouldSetStartFromDateTime()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureStartProperty<AllureTestResult>(DateTimeOffset.FromUnixTimeMilliseconds(100400))
        );

        await Assert.That(testResult.Start).IsEqualTo(100400);
    }

    [Test]
    public async Task ShouldSetStopFromNumber()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureStopProperty<AllureTestResult>(100)
        );

        await Assert.That(testResult.Stop).IsEqualTo(100);
    }

    [Test]
    public async Task ShouldSetStopFromDateTime()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureStopProperty<AllureTestResult>(DateTimeOffset.FromUnixTimeMilliseconds(100400))
        );

        await Assert.That(testResult.Stop).IsEqualTo(100400);
    }

    [Test]
    public async Task ShouldSetStopFromDurationNumber()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureStartProperty<AllureTestResult>(1),
            new AllureDurationProperty<AllureTestResult>(100)
        );

        await Assert.That(testResult.Stop).IsEqualTo(101);
    }

    [Test]
    public async Task ShouldSetStopFromDurationTimeSpan()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureStartProperty<AllureTestResult>(1),
            new AllureDurationProperty<AllureTestResult>(TimeSpan.FromMilliseconds(100))
        );

        await Assert.That(testResult.Stop).IsEqualTo(101);
    }

    [Test]
    public async Task ShouldSetStartFromDurationNumber()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureStopProperty<AllureTestResult>(101),
            new AllureDurationProperty<AllureTestResult>(100)
            {
                RelativeTo = AllureDurationAnchor.Stop
            }
        );

        await Assert.That(testResult.Start).IsEqualTo(1);
    }

    [Test]
    public async Task ShouldSetStartFromDurationTimeSpan()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureStopProperty<AllureTestResult>(101),
            new AllureDurationProperty<AllureTestResult>(TimeSpan.FromMilliseconds(100))
            {
                RelativeTo = AllureDurationAnchor.Stop
            }
        );

        await Assert.That(testResult.Start).IsEqualTo(1);
    }

    [Test]
    [Arguments(Status.Passed)]
    [Arguments(Status.Failed)]
    [Arguments(Status.Broken)]
    [Arguments(Status.Skipped)]
    public async Task ShouldSetStatus(Status expectedStatus)
    {
        var testResult = await this.ArrangeAndAct(
            new AllureStatusProperty<AllureTestResult>(expectedStatus)
        );

        await Assert.That(testResult.Status).IsEqualTo(expectedStatus);
    }

    [Test]
    public async Task ShouldOverwriteStatusByDefault()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureStatusProperty<AllureTestResult>(Status.Failed),
            new AllureStatusProperty<AllureTestResult>(Status.Passed)
        );

        await Assert.That(testResult.Status).IsEqualTo(Status.Passed);
    }

    [Test]
    public async Task ShouldNotOverwriteAlreadySetStatusIfOptedOut()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureStatusProperty<AllureTestResult>(Status.Failed),
            new AllureStatusProperty<AllureTestResult>(Status.Passed)
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
            new AllureStatusProperty<AllureTestResult>(Status.Passed)
            {
                OnlyIfUnset = true
            }
        );

        await Assert.That(testResult.Status).IsEqualTo(Status.Passed);
    }

    [Test]
    public async Task ShouldSetStatusDetails()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureStatusDetailsProperty<AllureTestResult>(new()
            {
                Message = "Foo",
                Trace = "Bar",
                Known = true,
                Muted = true,
            })
        );

        await Assert.That(testResult.StatusDetails.Message).IsEqualTo("Foo");
        await Assert.That(testResult.StatusDetails.Trace).IsEqualTo("Bar");
        await Assert.That(testResult.StatusDetails.Known).IsTrue();
        await Assert.That(testResult.StatusDetails.Muted).IsTrue();
    }

    [Test]
    public async Task ShouldSetBrokenStatusAndDetailsFromError()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureExceptionProperty<AllureTestResult>(new Exception("Foo"))
        );

        await Assert.That(testResult.Status).IsEqualTo(Status.Broken);
        await Assert.That(testResult.StatusDetails.Message).IsEqualTo("Foo");
        await Assert.That(testResult.StatusDetails.Trace).Contains("System.Exception");
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

        await Assert.That(testResult.Parameters).Count().IsEqualTo(4);
        var parameter1 = testResult.Parameters[0];
        var parameter2 = testResult.Parameters[1];
        var parameter3 = testResult.Parameters[2];
        var parameter4 = testResult.Parameters[3];

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
        var testResult = await this.ArrangeAndAct(
            new AllureParametersProperty<AllureTestResult>(
                [
                    new(){ Name = "foo", Value = "1" },
                    new(){ Name = "bar", Value = "2" },
                ]
            )
        );

        await Assert.That(testResult.Parameters).Count().IsEqualTo(2);
        var parameter1 = testResult.Parameters[0];
        var parameter2 = testResult.Parameters[1];

        await Assert.That(parameter1.Name).IsEqualTo("foo");
        await Assert.That(parameter1.Value).IsEqualTo("1");

        await Assert.That(parameter2.Name).IsEqualTo("bar");
        await Assert.That(parameter2.Value).IsEqualTo("2");
    }

    [Test]
    public async Task ShouldAddAllureAttachment()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureAttachmentProperty<AllureTestResult>(
                "Foo",
                new MemoryStream([1, 2, 3, 4])
            )
        );

        var attachment = await Assert.That(testResult.Attachments).HasSingleItem();
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
        var testResult = await this.ArrangeAndAct(
            new AllureAttachmentProperty<AllureTestResult>(
                "Foo",
                new MemoryStream([1, 2, 3, 4])
            )
            {
                MediaType = "application/json"
            }
        );

        var attachment = await Assert.That(testResult.Attachments).HasSingleItem();
        await Assert.That(attachment.Type).IsEqualTo("application/json");
    }

    [Test]
    public async Task ShouldAppendExtensionToDestinationFileName()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureAttachmentProperty<AllureTestResult>(
                "Foo",
                new MemoryStream([1, 2, 3, 4])
            )
            {
                FileExtension = ".txt"
            }
        );

        var attachment = await Assert.That(testResult.Attachments).HasSingleItem();
        await Assert.That(attachment.Source).EndsWith(".txt");
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

        var attachment = await Assert.That(testResult.Attachments).HasSingleItem();
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
        var testResult = await this.ArrangeAndAct(
            new AllureAttachmentFileProperty<AllureTestResult>(
                "Foo",
                "filepath"
            )
            {
                MediaType = "application/json"
            }
        );

        var attachment = await Assert.That(testResult.Attachments).HasSingleItem();
        await Assert.That(attachment.Type).IsEqualTo("application/json");
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

        var attachment = await Assert.That(testResult.Attachments).HasSingleItem();
        await Assert.That(attachment.Source).EndsWith(".txt");
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

        var attachment = await Assert.That(testResult.Attachments).HasSingleItem();
        await Assert.That(attachment.Source).EndsWith(".txt");
    }

    [Test]
    public async Task ShouldAddLabels()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureLabelsProperty([
                new(){ Name = "foo", Value = "bar" },
                new(){ Name = "baz", Value = "qux" },
            ])
        );

        await Assert.That(testResult.Labels)
            .Contains(l => l.Name == "foo" && l.Value == "bar")
            .And.Contains(l => l.Name == "baz" && l.Value == "qux");
    }

    [Test]
    public async Task ShouldAddLinks()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureLinksProperty([
                new(){ Name = "foo", Url = "bar" },
                new(){ Name = "baz", Url = "qux", Type = "qut" },
            ])
        );

        await Assert.That(testResult.Links)
            .Contains(l => l.Name == "foo" && l.Url == "bar" && l.Type is null)
            .And.Contains(l => l.Name == "baz" && l.Url == "qux" && l.Type == "qut");
    }

    [Test]
    public async Task ShouldAddDefaultSuites()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureDefaultSuitesProperty("foo", "bar", "baz")
        );

        var parentSuite = await Assert.That(testResult.Labels).HasSingleItem(l => l.Name == "parentSuite");
        var suite = await Assert.That(testResult.Labels).HasSingleItem(l => l.Name == "suite");
        var subSuite = await Assert.That(testResult.Labels).HasSingleItem(l => l.Name == "subSuite");
        await Assert.That(parentSuite.Value).IsEqualTo("foo");
        await Assert.That(suite.Value).IsEqualTo("bar");
        await Assert.That(subSuite.Value).IsEqualTo("baz");
    }

    [Test]
    public async Task ShouldAddDefaultSuitesFromType()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureDefaultSuitesProperty(typeof(TargetClass))
        );

        var parentSuite = await Assert.That(testResult.Labels).HasSingleItem(l => l.Name == "parentSuite");
        var suite = await Assert.That(testResult.Labels).HasSingleItem(l => l.Name == "suite");
        var subSuite = await Assert.That(testResult.Labels).HasSingleItem(l => l.Name == "subSuite");
        await Assert.That(parentSuite.Value).IsEqualTo("Allure.TestingPlatform.Tests");
        await Assert.That(suite.Value).IsEqualTo("Allure.TestingPlatform.Tests");
        await Assert.That(subSuite.Value).IsEqualTo("TargetClass");
    }

    [Test]
    public async Task ShouldRespectAllureNameAttributeWhenAddingDefaultSubSuitesFromType()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureDefaultSuitesProperty(typeof(TargetClassNamed))
        );

        await Assert.That(testResult.Labels)
            .Contains(l => l.Name == "subSuite" && l.Value == "Foo");
    }

    [Test]
    public async Task ShouldNotAddDefaultSuitesIfParentSuitePresent()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureLabelsProperty([new(){ Name = "parentSuite", Value = "qux" }]),
            new AllureDefaultSuitesProperty("foo", "bar", "baz")
        );

        var parentSuite = await Assert.That(testResult.Labels).HasSingleItem(l => l.Name == "parentSuite");
        await Assert.That(parentSuite.Value).IsEqualTo("qux");
        await Assert.That(testResult.Labels)
            .DoesNotContain(l => l.Name == "suite")
            .And.DoesNotContain(l => l.Name == "subSuite");
    }

    [Test]
    public async Task ShouldNotAddDefaultSuitesIfSuitePresent()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureLabelsProperty([new(){ Name = "suite", Value = "qux" }]),
            new AllureDefaultSuitesProperty("foo", "bar", "baz")
        );

        var suite = await Assert.That(testResult.Labels).HasSingleItem(l => l.Name == "suite");
        await Assert.That(suite.Value).IsEqualTo("qux");
        await Assert.That(testResult.Labels)
            .DoesNotContain(l => l.Name == "parentSuite")
            .And.DoesNotContain(l => l.Name == "subSuite");
    }

    [Test]
    public async Task ShouldNotAddDefaultSuitesIfSubSuitePresent()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureLabelsProperty([new(){ Name = "subSuite", Value = "qux" }]),
            new AllureDefaultSuitesProperty("foo", "bar", "baz")
        );

        var subSuite = await Assert.That(testResult.Labels).HasSingleItem(l => l.Name == "subSuite");
        await Assert.That(subSuite.Value).IsEqualTo("qux");
        await Assert.That(testResult.Labels)
            .DoesNotContain(l => l.Name == "parentSuite")
            .And.DoesNotContain(l => l.Name == "suite");
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

        await Assert.That(testResult.FullName).IsEqualTo(
            "Allure.TestingPlatform.Tests:Allure.TestingPlatform.Tests.TargetClass.TargetMethod(System.Int32,System.Int32,System.String,System.Int32,System.Int32,System.Int32)"
        );
        await Assert.That(testResult.TitlePath).IsEquivalentTo([
            "Allure.TestingPlatform.Tests",
            "Allure",
            "TestingPlatform",
            "Tests",
            "TargetClass",
            "TargetMethod(System.Int32,System.Int32,System.String,System.Int32,System.Int32,System.Int32)",
        ]);
        await Assert.That(testResult.Labels)
            .Contains(l => l.Name == "testClass" && l.Value == "TargetClass")
            .And.Contains(l => l.Name == "testMethod" && l.Value == "TargetMethod")
            .And.Contains(l => l.Name == "package" && l.Value == "Allure.TestingPlatform.Tests.TargetClass");
        await Assert.That(testResult.Parameters).IsEquivalentTo(
            [
                new Parameter() { Name = "p1", Value = "1" },
                new Parameter() { Name = "Foo", Value = "2" },
                new Parameter() { Name = "p3", Value = "\"foo\"", Mode = ParameterMode.Masked },
                new Parameter() { Name = "p4", Value = "3", Mode = ParameterMode.Hidden },
                new Parameter() { Name = "p5", Value = "4", Excluded = true },
            ],
            ParameterComparer.Instance,
            TUnit.Assertions.Enums.CollectionOrdering.Matching
        );

        await Assert.That(testResult.Labels)
            .Contains(l => l.Name == "epic" && l.Value == "foo")
            .And.Contains(l => l.Name == "feature" && l.Value == "bar");
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

        await Assert.That(testResult.TitlePath).IsEquivalentTo([
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

        await Assert.That(testResult.TitlePath).IsEquivalentTo([
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
                UpdateTargets = AllureTestMethodUpdateTargets.All & (~AllureTestMethodUpdateTargets.FullName),
            }
        );

        await Assert.That(testResult.FullName).IsEqualTo("1");
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
                UpdateTargets = AllureTestMethodUpdateTargets.All & (~AllureTestMethodUpdateTargets.TitlePath),
            }
        );

        await Assert.That(testResult.TitlePath).IsEmpty();
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
                UpdateTargets = AllureTestMethodUpdateTargets.All & (~AllureTestMethodUpdateTargets.Labels),
            }
        );

        await Assert.That(testResult.Labels)
            .DoesNotContain(l => l.Name == "testClass")
            .And.DoesNotContain(l => l.Name == "testMethod")
            .And.DoesNotContain(l => l.Name == "package");
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
                UpdateTargets = AllureTestMethodUpdateTargets.All & (~AllureTestMethodUpdateTargets.Parameters),
            }
        );

        await Assert.That(testResult.Parameters).IsEmpty();
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
                UpdateTargets = AllureTestMethodUpdateTargets.All & (~AllureTestMethodUpdateTargets.ApiAttributes),
            }
        );

        await Assert.That(testResult.Labels)
            .DoesNotContain(l => l.Name == "epic")
            .And.DoesNotContain(l => l.Name == "feature");
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

        await Assert.That(testResult.Parameters).IsEquivalentTo(
            [
                new Parameter() { Name = "p1", Value = "1" },
                new Parameter() { Name = "Foo", Value = "2" },
                new Parameter() { Name = "p3", Value = "\"foo\"", Mode = ParameterMode.Masked },
                new Parameter() { Name = "p4", Value = "3", Mode = ParameterMode.Hidden },
                new Parameter() { Name = "p5", Value = "4", Excluded = true },
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

        await Assert.That(testResult.FullName).IsEqualTo("foo");
    }

    [Test]
    public async Task ShouldApplyTitlePath()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureTitlePathProperty(["foo", "bar"])
        );

        await Assert.That(testResult.TitlePath).IsEquivalentTo(
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
