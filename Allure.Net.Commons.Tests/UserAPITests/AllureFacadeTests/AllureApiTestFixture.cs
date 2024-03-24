using Allure.Net.Commons.Tests.AssertionHelpers;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserAPITests.AllureFacadeTests;

class AllureApiTestFixture
{
    protected InMemoryResultsWriter writer;
    protected AllureLifecycle lifecycle;

    protected AllureContext Context => this.lifecycle.Context;

    [SetUp]
    public void SetUp()
    {
        this.writer = new InMemoryResultsWriter();
        this.lifecycle = new AllureLifecycle(_ => this.writer);
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
            Is.EqualTo(expectedLabels).Using(new LabelEqualityComparer())
        );
    }

    protected void AssertLinks(params Link[] expectedLinks)
    {
        Assert.That(
            this.lifecycle.Context.CurrentTest.links,
            Is.EqualTo(expectedLinks).Using(new LinkEqualityComparer())
        );
    }

    protected void AssertParameters(params Parameter[] expectedParameters)
    {
        Assert.That(
            this.lifecycle.Context.CurrentTest.parameters,
            Is.EqualTo(expectedParameters).Using(new ParameterEqualityComparer())
        );
    }
}
