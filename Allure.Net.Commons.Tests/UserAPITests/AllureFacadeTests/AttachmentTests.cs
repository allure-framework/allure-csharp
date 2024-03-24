using System;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserAPITests.AllureFacadeTests;

internal class AttachmentTests : AllureApiTestFixture
{
    [Test]
    public void ScreenDiffTest()
    {
        this.lifecycle.StartTestCase(new() { uuid = "1", fullName = "n" });
        var expectedExpected = File.ReadAllBytes("expected.png");
        var expectedActual = File.ReadAllBytes("actual.png");
        var expectedDiff = File.ReadAllBytes("diff.png");

        AllureApi.AddScreenDiff("expected.png", "actual.png", "diff.png");

        var attachment = this.Context.CurrentTest.attachments.Single();
        var content = JsonConvert.DeserializeAnonymousType(
            Encoding.UTF8.GetString(
                this.writer.attachments.Single().Content
            ),
            new { expected = "", actual = "", diff = "" }
        );
        var prefix = "data:image/png;base64,";
        var actualExpected = Convert.FromBase64String(
            content.expected[prefix.Length..]
        );
        var actualActual = Convert.FromBase64String(
            content.actual[prefix.Length..]
        );
        var actualDiff = Convert.FromBase64String(
            content.diff[prefix.Length..]
        );

        Assert.That(attachment.name, Is.EqualTo("diff-1"));
        Assert.That(attachment.type, Is.EqualTo("application/vnd.allure.image.diff"));
        Assert.That(attachment.source, Does.EndWith(".json"));
        Assert.That(content.expected, Does.StartWith(prefix));
        Assert.That(content.actual, Does.StartWith(prefix));
        Assert.That(content.diff, Does.StartWith(prefix));
        Assert.That(actualExpected, Is.EqualTo(expectedExpected));
        Assert.That(actualActual, Is.EqualTo(expectedActual));
        Assert.That(actualDiff, Is.EqualTo(expectedDiff));
    }

    [Test]
    public void ScreenDiffNameIncremented()
    {
        this.lifecycle.StartTestCase(new() { uuid = "1", fullName = "n" });
        AllureApi.AddScreenDiff("expected.png", "actual.png", "diff.png");

        AllureApi.AddScreenDiff("expected.png", "actual.png", "diff.png");

        var name = this.Context.CurrentTest.attachments.Last().name;
        Assert.That(name, Is.EqualTo("diff-2"));
    }

    [Test]
    public void StepScreenDiff()
    {
        this.lifecycle.StartTestCase(new() { uuid = "1", fullName = "n" });
        ExtendedApi.StartStep("step");

        AllureApi.AddScreenDiff("expected.png", "actual.png", "diff.png");

        Assert.That(
            this.Context.CurrentStep.attachments,
            Is.Not.Empty
        );
    }

    [Test]
    public void FixtureScreenDiff()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "2" });
        ExtendedApi.StartBeforeFixture("fixture");

        AllureApi.AddScreenDiff("expected.png", "actual.png", "diff.png");

        Assert.That(
            this.Context.CurrentFixture.attachments,
            Is.Not.Empty
        );
    }
}
