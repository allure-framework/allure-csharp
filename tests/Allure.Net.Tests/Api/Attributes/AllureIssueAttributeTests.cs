using System.Reflection;
using TestResult = Allure.Model.TestResult;

namespace Allure.Net.Tests.Api.Attributes;

public class AllureIssueAttributeTests
{
    [Test]
    public async Task CanBeQueriedFromBaseAndInheritedClasses()
    {
        var attrs = typeof(Derived).GetCustomAttributes<AllureIssueAttribute>();
        await Assert.That(attrs.Count()).IsEqualTo(2);
    }

    [Test]
    public async Task IssueCanBeAddedToTestWithOptionalTitle()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllureIssueAttribute("ISSUE-17") { Title = "Issue title" };

        attr.Apply(tr);

        await Assert.That(attr.IdOrUrl).IsEqualTo("ISSUE-17");
        await AttributeAssertions.AssertLink(tr, "ISSUE-17", "Issue title", "issue");
    }

    [Test]
    public async Task IssueWithoutTitleCanBeAddedToTest()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllureIssueAttribute("ISSUE-17");

        attr.Apply(tr);

        await Assert.That(attr.IdOrUrl).IsEqualTo("ISSUE-17");
        await AttributeAssertions.AssertLink(tr, "ISSUE-17", null, "issue");
    }

    [Test]
    public async Task DoesNothingIfUrlIsNull()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };

        new AllureIssueAttribute(null!).Apply(tr);

        await Assert.That(tr.Links).IsEmpty();
    }

    [AllureIssue("BASE-1")]
    private class Base;
    [AllureIssue("DERIVED-1")]
    private class Derived : Base;
}
