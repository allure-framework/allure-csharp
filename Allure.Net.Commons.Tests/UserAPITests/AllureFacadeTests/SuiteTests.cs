using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserAPITests.AllureFacadeTests;
class SuiteTests : AllureApiTestFixture
{
    [Test]
    public void ParentSuiteCanBeAdded()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.AddParentSuite("My Parent Suite");

        this.AssertLabels(
            new Label() { name = "parentSuite", value = "My Parent Suite" }
        );
    }

    [Test]
    public void AddParentSuiteAppendsLabel()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });
        AllureApi.AddParentSuite("My Parent Suite 1");

        AllureApi.AddParentSuite("My Parent Suite 2");

        this.AssertLabels(
            new Label() { name = "parentSuite", value = "My Parent Suite 1" },
            new Label() { name = "parentSuite", value = "My Parent Suite 2" }
        );
    }

    [Test]
    public void ParentSuiteCanBeSet()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.SetParentSuite("My Parent Suite");

        this.AssertLabels(
            new Label() { name = "parentSuite", value = "My Parent Suite" }
        );
    }

    [Test]
    public void SetParentSuiteOverwritesLabel()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });
        AllureApi.AddParentSuite("My Parent Suite 1");

        AllureApi.SetParentSuite("My Parent Suite 2");

        this.AssertLabels(
            new Label() { name = "parentSuite", value = "My Parent Suite 2" }
        );
    }

    [Test]
    public void SuiteCanBeAdded()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.AddSuite("My Suite");

        this.AssertLabels(
            new Label() { name = "suite", value = "My Suite" }
        );
    }

    [Test]
    public void AddSuiteAppendsLabel()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });
        AllureApi.AddSuite("My Suite 1");

        AllureApi.AddSuite("My Suite 2");

        this.AssertLabels(
            new Label() { name = "suite", value = "My Suite 1" },
            new Label() { name = "suite", value = "My Suite 2" }
        );
    }

    [Test]
    public void SuiteCanBeSet()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.SetSuite("My Suite");

        this.AssertLabels(
            new Label() { name = "suite", value = "My Suite" }
        );
    }

    [Test]
    public void SetSuiteOverwritesLabel()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });
        AllureApi.AddSuite("My Suite 1");

        AllureApi.SetSuite("My Suite 2");

        this.AssertLabels(
            new Label() { name = "suite", value = "My Suite 2" }
        );
    }

    [Test]
    public void SubSuiteCanBeAdded()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.AddSubSuite("My Sub-suite");

        this.AssertLabels(
            new Label() { name = "subSuite", value = "My Sub-suite" }
        );
    }

    [Test]
    public void AddSubSuiteAppendsLabel()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });
        AllureApi.AddSubSuite("My Sub-suite 1");

        AllureApi.AddSubSuite("My Sub-suite 2");

        this.AssertLabels(
            new Label() { name = "subSuite", value = "My Sub-suite 1" },
            new Label() { name = "subSuite", value = "My Sub-suite 2" }
        );
    }

    [Test]
    public void SubSuiteCanBeSet()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.SetSubSuite("My Sub-suite");

        this.AssertLabels(
            new Label() { name = "subSuite", value = "My Sub-suite" }
        );
    }

    [Test]
    public void SetSubSuiteOverwritesLabel()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });
        AllureApi.AddSubSuite("My Sub-suite 1");

        AllureApi.SetSubSuite("My Sub-suite 2");

        this.AssertLabels(
            new Label() { name = "subSuite", value = "My Sub-suite 2" }
        );
    }
}
