using Allure.Net.Commons.Tests.AssertionHelpers;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserAPITests.AllureFacadeTests;

class AllureApiTestFixture
{
    protected AllureLifecycle lifecycle;

    protected AllureContext Context => this.lifecycle.Context;

    [SetUp]
    public void SetUp()
    {
        this.lifecycle = new AllureLifecycle();
        AllureApi.CurrentLifecycle = lifecycle;
    }

    [TearDown]
    public void TearDown()
    {
        AllureApi.CurrentLifecycle = null;
    }

    protected void AssertLabels(params Label[] expectedLabels)
    {
        Assert.That(
            this.lifecycle.Context.CurrentTest.labels,
            Is.EqualTo(expectedLabels).Using(new LabelsEqualityComparer())
        );
    }

    protected void AssertLinks(params Link[] expectedLinks)
    {
        Assert.That(
            this.lifecycle.Context.CurrentTest.links,
            Is.EqualTo(expectedLinks).Using(new LinksEqualityComparer())
        );
    }
}
