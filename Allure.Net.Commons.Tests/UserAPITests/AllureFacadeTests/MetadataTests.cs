using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserAPITests.AllureFacadeTests;

class MetadataTests : AllureApiTestFixture
{
    [Test]
    public void TestTitleCanBeChanged()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.SetName("Test's title");

        Assert.That(this.Context.CurrentTest.name, Is.EqualTo("Test's title"));
    }

    [Test]
    public void FixtureTitleCanBeChanged()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });
        this.lifecycle.StartBeforeFixture(new());

        AllureApi.SetName("Fixture's title");

        Assert.That(this.Context.CurrentFixture.name, Is.EqualTo("Fixture's title"));
    }

    [Test]
    public void TestDescriptionCanBeSet()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.SetDescription("Test's description");

        Assert.That(this.Context.CurrentTest.description, Is.EqualTo("Test's description"));
    }

    [Test]
    public void TestDescriptionHtmlCanBeSet()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.SetDescriptionHtml("Test's description");

        Assert.That(this.Context.CurrentTest.descriptionHtml, Is.EqualTo("Test's description"));
    }

    [Test]
    public void LabelsCanBeAdded()
    {
        var testResult = new TestResult() { uuid = "uuid" };
        testResult.labels.Add(new() { name = "l1", value = "v1" });
        this.lifecycle.StartTestCase(testResult);

        AllureApi.AddLabels(
            new() { name = "l2", value = "v2" },
            new() { name = "l3", value = "v3" }
        );

        this.AssertLabels(
            new() { name = "l1", value = "v1" },
            new() { name = "l2", value = "v2" },
            new() { name = "l3", value = "v3" }
        );
    }

    [Test]
    public void NewLabelCanBeAddedByNameValue()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.AddLabel("l", "v");

        this.AssertLabels(
            new Label() { name = "l", value = "v" }
        );
    }

    [Test]
    public void LabelsOverwriteByNameValue()
    {
        var testResult = new TestResult() { uuid = "uuid" };
        testResult.labels.Add(new() { name = "l1", value = "v1" });
        testResult.labels.Add(new() { name = "l2", value = "v2" });
        testResult.labels.Add(new() { name = "l2", value = "v3" });
        this.lifecycle.StartTestCase(testResult);

        AllureApi.AddLabel("l2", "v4", overwrite: true);

        this.AssertLabels(
            new() { name = "l1", value = "v1" },
            new() { name = "l2", value = "v4" }
        );
    }

    [Test]
    public void NewLabelCanBeAdded()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.AddLabel(new() { name = "l", value = "v" });

        this.AssertLabels(
            new Label() { name = "l", value = "v" }
        );
    }

    [Test]
    public void LabelsOverwrite()
    {
        var testResult = new TestResult() { uuid = "uuid" };
        testResult.labels.Add(new() { name = "l1", value = "v1" });
        testResult.labels.Add(new() { name = "l2", value = "v2" });
        testResult.labels.Add(new() { name = "l2", value = "v3" });
        this.lifecycle.StartTestCase(testResult);

        AllureApi.AddLabel(new() { name = "l2", value = "v4" }, overwrite: true);

        this.AssertLabels(
            new() { name = "l1", value = "v1" },
            new() { name = "l2", value = "v4" }
        );
    }

    [Test]
    public void SeverityAdded()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });


        AllureApi.SetSeverity(SeverityLevel.critical);

        this.AssertLabels(
            new Label() { name = "severity", value = "critical" }
        );
    }

    [Test]
    public void SeverityOverwritesPreviousValue()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });
        AllureApi.SetSeverity(SeverityLevel.minor);

        AllureApi.SetSeverity(SeverityLevel.critical);

        this.AssertLabels(
            new Label() { name = "severity", value = "critical" }
        );
    }

    [Test]
    public void OwnerAdded()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });


        AllureApi.SetOwner("John");

        this.AssertLabels(
            new Label() { name = "owner", value = "John" }
        );
    }

    [Test]
    public void OwnerOverwritesPreviousValue()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });
        AllureApi.SetOwner("Jane");

        AllureApi.SetOwner("John");

        this.AssertLabels(
            new Label() { name = "owner", value = "John" }
        );
    }

    [Test]
    public void AllureIdAdded()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });


        AllureApi.SetAllureId(1234);

        this.AssertLabels(
            new Label() { name = "ALLURE_ID", value = "1234" }
        );
    }

    [Test]
    public void AllureIdOverwritesPreviousValue()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });
        AllureApi.SetAllureId(1234);

        AllureApi.SetAllureId(5678);

        this.AssertLabels(
            new Label() { name = "ALLURE_ID", value = "5678" }
        );
    }

    [Test]
    public void TagsCanBeAdded()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.AddTags("tag1", "tag2");

        this.AssertLabels(
            new() { name = "tag", value = "tag1" },
            new() { name = "tag", value = "tag2" }
        );
    }
}
