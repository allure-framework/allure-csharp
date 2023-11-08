using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserAPITests.AllureFacadeTests;

class LinkTests : AllureApiTestFixture
{
    [Test]
    public void LinkCanBeAdded()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.AddLink("https://domain.com");

        this.AssertLinks(
            new Link() { url = "https://domain.com" }
        );
    }

    [Test]
    public void NamedLinkCanBeAdded()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.AddLink("link-name", "https://domain.com");

        this.AssertLinks(
            new Link() { url = "https://domain.com", name = "link-name" }
        );
    }

    [Test]
    public void NamedLinkWithTypeCanBeAdded()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.AddLink("link-name", "link-type", "https://domain.com");

        this.AssertLinks(
            new Link()
            {
                url = "https://domain.com",
                name = "link-name",
                type = "link-type"
            }
        );
    }

    [Test]
    public void IssueLinkCanBeAdded()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.AddIssue("https://domain.com");

        this.AssertLinks(
            new Link()
            {
                url = "https://domain.com",
                type = "issue"
            }
        );
    }

    [Test]
    public void NamedIssueLinkCanBeAdded()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.AddIssue("issue-1", "https://domain.com");

        this.AssertLinks(
            new Link()
            {
                name = "issue-1",
                url = "https://domain.com",
                type = "issue"
            }
        );
    }

    [Test]
    public void TmsItemLinkCanBeAdded()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.AddTmsItem("https://domain.com");

        this.AssertLinks(
            new Link()
            {
                url = "https://domain.com",
                type = "tms"
            }
        );
    }

    [Test]
    public void NamedTmsItemLinkCanBeAdded()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.AddTmsItem("testcase-1", "https://domain.com");

        this.AssertLinks(
            new Link()
            {
                name = "testcase-1",
                url = "https://domain.com",
                type = "tms"
            }
        );
    }

    [Test]
    public void LinkInstancesCanBeAdded()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.AddLinks(
            new()
            {
                url = "https://domain1.com",
                name = "link-name1",
                type = "link-type1"
            },
            new()
            {
                url = "https://domain2.com",
                name = "link-name2",
                type = "link-type2"
            }
        );

        this.AssertLinks(
            new Link()
            {
                url = "https://domain1.com",
                name = "link-name1",
                type = "link-type1"
            },
            new Link()
            {
                url = "https://domain2.com",
                name = "link-name2",
                type = "link-type2"
            }
        );
    }
}
